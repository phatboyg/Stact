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
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using Magnum.Logging;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Sending_a_message_to_a_remote_channel_via_wcf
	{
		[Test]
		public void Should_properly_arrive_at_the_destination()
		{
			TraceLogProvider.Configure(LogLevel.Debug);
			_log = Logger.GetLogger<Sending_a_message_to_a_remote_channel_via_wcf>();
			_log.Debug("Starting");

			var serviceUri = new Uri("net.pipe://localhost/pipe");
			string pipeName = "test";

			var future = new Future<TestMessage>();
			var message = new TestMessage
				{
					Id = Guid.NewGuid(),
					Name = "Alpha",
				};

			using (var remote = new WcfUntypedChannelAdapter(new SynchronousFiber(), serviceUri, pipeName))
			{
				_log.Debug("Remote channel adapter created");
				using (remote.Subscribe(x =>
					{
						x.Consume<TestMessage>()
							.Using(m => future.Complete(m));
					}))
				{
					var client = new WcfUntypedChannel(new SynchronousFiber(), serviceUri, pipeName);
					_log.Debug("Client created");

					client.Send(message);

					future.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
				}
			}

			future.Value.ShouldNotBeNull();
			future.Value.ShouldEqual(message);
			future.Value.ShouldNotBeTheSameAs(message);
		}


		private ILogger _log;

		private class TestMessage
		{
			public Guid Id { get; set; }
			public string Name { get; set; }

			public bool Equals(TestMessage other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return other.Id.Equals(Id) && Equals(other.Name, Name);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (TestMessage)) return false;
				return Equals((TestMessage) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (Id.GetHashCode()*397) ^ (Name != null ? Name.GetHashCode() : 0);
				}
			}
		}
	}
}