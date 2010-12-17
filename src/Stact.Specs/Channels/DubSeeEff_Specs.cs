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
namespace Stact.Specs.Channels
{
	using System;
	using System.Runtime.Serialization;
	using Internal;
	using Magnum.TestFramework;
	using Stact;
	using Magnum.Extensions;
	using NUnit.Framework;


	[TestFixture]
	public class Sending_a_message_through_a_wcf_channel
	{
		[Test]
		[Category("Slow")]
		public void Should_property_adapt_itself_to_a_channel_network()
		{
			var serviceUri = new Uri("net.pipe://localhost/Pipe");
			string pipeName = "Test";
			Channel<TestMessage> adapter = new ChannelAdapter<TestMessage>();
			using (var host = new WcfChannelHost<TestMessage>(adapter, serviceUri, pipeName))
			{
				var future = new Future<TestMessage>();

				using (adapter.Connect(x =>
					{
						x.AddConsumer(future.Complete)
							.HandleOnCallingThread();
					}))
				{
					var client = new WcfChannelProxy<TestMessage>(new SynchronousFiber(), serviceUri, pipeName);

					client.Send(new TestMessage("Hello!"));

					future.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
				}
			}
		}


		[DataContract]
		public class TestMessage
		{
			public TestMessage(string s)
			{
				Value = s;
			}

			[DataMember]
			public string Value { get; set; }

			public override string ToString()
			{
				return Value;
			}
		}
	}
}