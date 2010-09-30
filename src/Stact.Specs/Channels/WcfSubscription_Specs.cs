// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Stact.Specs.Channels
{
	using System;
	using System.Linq;
	using Stact.Channels;
	using Stact.Extensions;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	[Category("Slow")]
	public class When_connecting_two_services_through_a_wcf_proxy_and_host
	{
		const string _pipeName = "test";
		ChannelConnection _inputConnection;
		Future<TestMessage> _consumer;
		ChannelAdapter _input;
		ChannelAdapter _output;
		ChannelConnection _outputConnection;
		Uri _pipeUri;

		[When]
		public void Connecting_two_services_through_a_wcf_proxy_and_host()
		{
			_pipeUri = new Uri("net.pipe://localhost/pipe");

			_consumer = new Future<TestMessage>();

			_input = new ChannelAdapter();
			_inputConnection = _input.Connect(x =>
				{
					x.ReceiveFromWcfChannel(_pipeUri, _pipeName);

					x.AddConsumerOf<TestMessage>()
						.UsingConsumer(_consumer.Complete);
				});

			_output = new ChannelAdapter();
			_outputConnection = _output.Connect(x =>
				{
					x.SendToWcfChannel(_pipeUri, _pipeName)
						.HandleOnCallingThread();
				});
		}

		[Then]
		public void Should_match_the_defined_channel_input_structure()
		{
			_input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
				{
					typeof(ChannelAdapter),
					typeof(BroadcastChannel),
					typeof(TypedChannelAdapter<TestMessage>),
					typeof(ConsumerChannel<TestMessage>),
				});
		}

		[Then]
		public void Should_match_the_defined_channel_output_structure()
		{
			_output.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
				{
					typeof(ChannelAdapter),
					typeof(BroadcastChannel),
					typeof(WcfChannelProxy),
				});
		}

		[Then]
		public void Should_get_the_message()
		{
			_output.Send(new TestMessage());

			_consumer.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
		}

		[After]
		public void After()
		{
			_inputConnection.Dispose();
			_inputConnection = null;
			_input = null;

			_outputConnection.Dispose();
			_outputConnection = null;
			_output = null;
		}


		class TestMessage
		{
		}
	}
}