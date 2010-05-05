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
	using System;
	using System.Runtime.Serialization;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using Magnum.Logging;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Sending_a_message_through_a_wcf_channel
	{
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

		[Test]
		public void Should_work()
		{
			TraceLogProvider.Configure(LogLevel.Debug);

			var log = Logger.GetLogger<Sending_a_message_through_a_wcf_channel>();

			log.Debug("Starting");
			var future = new Future<TestMessage>();
			Channel<TestMessage> channel = new ConsumerChannel<TestMessage>(new SynchronousFiber(), x =>
				{
					log.Debug(l => l.Write("Received: {0}", x.Value));
					future.Complete(x);
				});

			var serviceUri = new Uri("net.pipe://localhost/Pipe");
			var pipeName = "Test";
			var host = new LocalWcfChannelServiceHost<TestMessage>(new SynchronousFiber(), serviceUri, pipeName, channel);

			log.Debug("Host started");
			try
			{
				var client = new LocalWcfChannelProxy<TestMessage>(new SynchronousFiber(), serviceUri, pipeName);
				log.Debug("Client started");

				client.Send(new TestMessage("Hello!"));

				future.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
				
				log.Debug("Complete");
			}
			finally
			{
				host.Stop();
			}
		}
	}
}