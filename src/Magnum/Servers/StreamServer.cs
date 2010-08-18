// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Servers
{
	using System;
	using System.Threading;
	using Channels;
	using Events;
	using Fibers;
	using Logging;
	using StateMachine;


	public abstract class StreamServer<T> :
		StateMachine<StreamServer<T>>
		where T : StreamServer<T>
	{
		static readonly ILogger _log = Logger.GetLogger<T>();
		readonly Uri _uri;

		int _connectionCount;

		static StreamServer()
		{
			Define(() =>
				{
					Initially(
					          When(Start)
					          	.Call(instance => instance.StartListener())
					          	.TransitionTo(Running)
						);

					During(Running,
					       When(Stop)
					       	.TransitionTo(Stopping));

					Anytime(
					        When(Starting.Enter)
					        	.Then(instance => instance.Publish<ServerStarting>()),
					        When(Running.Enter)
					        	.Call(instance => instance.Publish<ServerRunning>()),
					        When(Stopping.Enter)
					        	.Call(instance => instance.Publish<ServerStopping>())
					        	.Call(instance => instance.StopListener()),
					        When(Stopped.Enter)
					        	.Call(instance => instance.Publish<ServerStopped>())
						);
				});
		}

		protected StreamServer(Uri uri, Fiber fiber, UntypedChannel eventChannel)
		{
			_uri = uri;
			Fiber = fiber;
			EventChannel = eventChannel;
		}

		public Uri BaseUri
		{
			get { return _uri; }
		}

		protected UntypedChannel EventChannel { get; private set; }
		protected Fiber Fiber { get; private set; }


		public static Event Start { get; set; }
		public static Event Stop { get; set; }

		public static State Initial { get; set; }
		public static State Starting { get; set; }
		public static State Running { get; set; }
		public static State Stopped { get; set; }
		public static State Stopping { get; set; }
		public static State Completed { get; set; }

		public int ConnectionCount
		{
			get { return _connectionCount; }
		}

		void StartListener()
		{
			StartListener(_uri);
		}

		protected virtual void StartListener(Uri uri)
		{
			ChangeCurrentState(Starting);
		}

		protected void Publish<T>()
			where T : ServerEvent, new()
		{
			EventChannel.Send(new T());
		}

		protected void ConnectionEstablished(Action handleConnectionAction)
		{
			Interlocked.Increment(ref _connectionCount);

			Fiber.Add(handleConnectionAction);
		}

		protected void ConnectionComplete()
		{
			int count = Interlocked.Decrement(ref _connectionCount);

			if (CurrentState == Stopping)
			{
				if (count == 0)
					ShutdownListener();
			}
		}

		void StopListener()
		{
			if (_connectionCount == 0)
				ShutdownListener();
		}

		protected virtual void ShutdownListener()
		{
			ChangeCurrentState(Stopped);
		}
	}


	public static class ExtensionsToSocketServer
	{
		public static void Start<T>(this StreamServer<T> server)
			where T : StreamServer<T>
		{
			server.RaiseEvent(StreamServer<T>.Start);
		}

		public static void Stop<T>(this StreamServer<T> server)
			where T : StreamServer<T>
		{
			server.RaiseEvent(StreamServer<T>.Stop);
		}
	}
}