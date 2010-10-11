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
namespace Stact.Specs.Headers
{
	using Internal;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[Scenario]
	public class When_sending_a_message_to_a_channel
	{
		UntypedChannel _channel;
		Future<Simple> _received;

		[SetUp]
		public void Sending_a_message_to_a_channel()
		{
			_received = new Future<Simple>();

			_channel = new ChannelAdapter();
			_channel.Connect(x =>
				{
					x.AddConsumerOf<Simple>()
						.UsingConsumer(_received.Complete)
						.HandleOnCallingThread();
				});
		}

		[Then]
		public void Should_receive_the_raw_message_type()
		{
			_channel.Send<Simple>(new SimpleImpl());

			_received.IsCompleted.ShouldBeTrue("Message was not received");
		}

		[Then]
		public void Should_receive_the_message_type()
		{
			_channel.Send<Message<Simple>>(new MessageImpl<Simple>(new SimpleImpl()));

			_received.IsCompleted.ShouldBeTrue("Message was not received");
		}

		[Then]
		public void Should_receive_the_request_message_type()
		{
			var responseChannel = new ChannelAdapter();

			_channel.Request<Simple>(new SimpleImpl(), responseChannel);

			_received.IsCompleted.ShouldBeTrue("Message was not received");
		}
	}


	public interface Simple
	{
	}


	public class SimpleImpl :
		Simple
	{
	}
}