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
	using System.Collections.Generic;
	using Fibers;
	using Stact.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using Magnum.TestFramework;

	[TestFixture]
	public class Sending_to_an_interval_subscriber
	{
		[Test]
		public void Should_deliver_the_messages_at_once()
		{
			var queue = new SynchronousFiber();
			var scheduler = new TimerScheduler(new SynchronousFiber());

			var called = new Future<ICollection<MyMessage>>();
			var consumer = new ConsumerChannel<ICollection<MyMessage>>(queue, called.Complete);

			var channel = new IntervalChannel<MyMessage>(queue, scheduler, 2.Seconds(), consumer);

			for (int i = 0; i < 5; i++)
			{
				channel.Send(new MyMessage());
			}

			called.WaitUntilCompleted(4.Seconds()).ShouldBeTrue();

			channel.Dispose();

			called.Value.ShouldNotBeNull();
			called.Value.Count.ShouldEqual(5);
		}
	}
}