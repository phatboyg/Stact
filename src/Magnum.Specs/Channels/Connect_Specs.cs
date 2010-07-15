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
namespace Magnum.Specs.Channels
{
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;


	[TestFixture]
	public class Using_the_connect_method
	{
		[Test]
		public void Should_allow_the_consumer_channel_type()
		{
			var input = new ChannelAdapter();

			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(message => { })
						.ExecuteOnThreadPoolFiber();
				}))
			{
			}
		}

		[Test]
		public void Should_allow_the_selective_consumer_channel_type()
		{
			var input = new ChannelAdapter();

			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.UsingSelectiveConsumer(message => m => { });
				}))
			{
			}
		}

		[Test]
		public void Should_allow_an_interval_channel_to_be_created()
		{
			var input = new ChannelAdapter();

			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.BufferFor(15.Seconds())
						.UsingConsumer(messages => { });
				}))
			{
			}
		}


		class TestMessage
		{
		}
	}
}