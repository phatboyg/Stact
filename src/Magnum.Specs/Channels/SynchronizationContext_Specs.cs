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
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;


	[TestFixture]
	public class Creating_a_channel_with_a_synchronization_context
	{
		[Test]
		public void Should_properly_wrap_the_channel_as_synchronized()
		{
			Assert.IsNull(SynchronizationContext.Current);

			var fiber = new ThreadPoolFiber();

			var input = new ChannelAdapter();

			var context = new TestSynchronizationContext();

			var future = new Future<TestMessage>();

			SynchronizationContext.SetSynchronizationContext(context);

			Assert.IsNotNull(SynchronizationContext.Current);

			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.OnCurrentSynchronizationContext()
						.UsingConsumer(message =>
							{
								Trace.WriteLine("Received on Thread: " + Thread.CurrentThread.ManagedThreadId);

								Assert.IsNotNull(SynchronizationContext.Current);
								Assert.AreEqual(context, SynchronizationContext.Current);

								future.Complete(message);
							});
				}))
			{
				Trace.WriteLine("Subscribed on Thread: " + Thread.CurrentThread.ManagedThreadId);

				SynchronizationContext.SetSynchronizationContext(null);

				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof(ChannelAdapter),
						typeof(BroadcastChannel),
						typeof(TypedChannelAdapter<TestMessage>),
						typeof(SynchronizedChannel<TestMessage>),
						typeof(ConsumerChannel<TestMessage>),
					});

				fiber.Add(() =>
					{
						Trace.WriteLine("Thread: " + Thread.CurrentThread.ManagedThreadId);
						Assert.IsNull(SynchronizationContext.Current);

						input.Send(new TestMessage());
					});

				Assert.IsNull(SynchronizationContext.Current);

				future.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
			}
		}


		class TestMessage
		{
		}


		class TestSynchronizationContext :
			SynchronizationContext
		{
			public override void Send(SendOrPostCallback d, object state)
			{
				SetSynchronizationContext(this);

				base.Send(d, state);
			}
		}
	}
}