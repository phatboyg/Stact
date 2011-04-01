// Copyright 2010 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Stact.ServerFramework.Specs
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Text;
	using Events;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[Scenario]
	public class When_starting_an_http_server
	{
		ChannelConnection _connection;
		ChannelAdapter _input;
		Future<ServerRunning> _runningEventReceived;
		HttpServer _server;
		Future<ServerStarting> _startingEventReceived;

		public Uri ServerUri { get; private set; }

		[When]
		public void Starting_a_socket_server()
		{
			_startingEventReceived = new Future<ServerStarting>();
			_runningEventReceived = new Future<ServerRunning>();

			_input = new ChannelAdapter();
			_connection = _input.Connect(x =>
				{
					x.AddConsumerOf<ServerStarting>()
						.UsingConsumer(_startingEventReceived.Complete)
						.HandleOnCallingThread();

					x.AddConsumerOf<ServerRunning>()
						.UsingConsumer(_runningEventReceived.Complete)
						.HandleOnCallingThread();
				});

			ServerUri = new Uri("http://localhost:8008/Topshelf");
			_server = new HttpServer(ServerUri, new PoolFiber(), _input, new[]
				{
					new VersionConnectionHandler(),
				});
			_server.Start();
		}

		[After]
		public void Finally()
		{
			_server.Stop();

			_connection.Dispose();
		}

		[Then]
		public void Should_be_available_to_accept_connections()
		{
			_server.CurrentState.ShouldEqual(HttpServer.Running);
		}

		[Then]
		public void Should_have_called_the_starting_event()
		{
			_startingEventReceived.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}

		[Then]
		public void Should_have_called_the_running_event()
		{
			_runningEventReceived.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}
	}


	[Scenario]
	[Explicit]
	public class When_connecting_to_an_http_server :
		When_starting_an_http_server
	{
		HttpWebRequest _webRequest;
		HttpWebResponse _webResponse;

		[When]
		public void Connecting_to_a_socket_server()
		{
			_webRequest = (HttpWebRequest)WebRequest.Create(ServerUri.AppendPath("version"));
			try
			{
				_webResponse = (HttpWebResponse)_webRequest.GetResponse();
			}
			catch (WebException ex)
			{
				_webResponse = (HttpWebResponse)ex.Response;
			}
		}

		[After]
		public void My_Finally()
		{
			using (_webResponse)
				_webResponse.Close();
		}

		[Then]
		public void Should_establish_a_connection()
		{
			using (Stream responseStream = _webResponse.GetResponseStream())
			{
				string response = responseStream.ReadToEndAsText();
				Trace.WriteLine(response);
			}
		}

		[Then]
		public void Should_allow_multiple_connections()
		{
			var requests = new List<HttpWebRequest>();

			Stopwatch connectionTimer = Stopwatch.StartNew();

			int expected = 100;
			for (int i = 0; i < expected; i++)
			{
				var webRequest = (HttpWebRequest)WebRequest.Create(ServerUri.AppendPath("Services/MyService"));
				webRequest.Method = "POST";
				using (Stream reque = webRequest.GetRequestStream())
				{
					byte[] buffer = Encoding.UTF8.GetBytes("Hello");

					reque.Write(buffer, 0, buffer.Length);
				}

				requests.Add(webRequest);
			}

			connectionTimer.Stop();

			Trace.WriteLine("Established {0} connections in {0}ms".FormatWith(expected, connectionTimer.ElapsedMilliseconds));

			requests.ForEach(request =>
				{
					try
					{
						using (WebResponse webResponse = request.GetResponse())
						{
							using (Stream responseStream = webResponse.GetResponseStream())
							{
								string response = responseStream.ReadToEndAsText();
								Trace.WriteLine(response);
							}
							webResponse.Close();
						}
					}
					catch (WebException ex)
					{
						using (WebResponse response = ex.Response)
						{
							var httpResponse = (HttpWebResponse)response;

							Trace.WriteLine("HttpStatusCode: " + httpResponse.StatusCode);
							using (Stream data = response.GetResponseStream())
							{
								string r = data.ReadToEndAsText();
								Trace.WriteLine(r);
							}
						}
					}
				});

			requests.Clear();
		}
	}
}