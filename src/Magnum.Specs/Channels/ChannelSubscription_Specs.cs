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
	using System.Collections.Generic;
	using System.Linq;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Subscribing_to_a_channel
	{
		private class TestMessage
		{
			public int Value { get; set; }
		}

		[Test]
		public void Should_be_an_distinct_interval_consumer_on_the_channel()
		{
			var future = new Future<int>();

			var input = new UntypedChannelAdapter(new SynchronousFiber());
			int expected = 5;
			using (input.Subscribe(x =>
				{
					x.Consume<TestMessage>()
						.Every(1.Seconds(), c => c.Value)
						.Using(message => { future.Complete(message.Count); });
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (UntypedChannelAdapter),
						typeof (UntypedChannelRouter),
						typeof (TypedChannelAdapter<TestMessage>),
						typeof (DistinctIntervalChannel<TestMessage, int>),
						typeof (ConsumerChannel<IDictionary<int,TestMessage>>),
					});

				for (int i = 0; i < expected; i++)
				{
					input.Send(new TestMessage { Value = i});
				}
			}

			future.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
			future.Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_be_an_empty_channel_adapter_with_no_consumers()
		{
			var input = new UntypedChannelAdapter(new SynchronousFiber());
			using (input.Subscribe(x => { }))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (UntypedChannelAdapter),
						typeof (ShuntChannel)
					});
			}
		}

		[Test]
		public void Should_be_an_interval_consumer_on_the_channel()
		{
			var future = new Future<int>();

			var input = new UntypedChannelAdapter(new SynchronousFiber());
			int expected = 5;
			using (input.Subscribe(x =>
				{
					x.Consume<TestMessage>()
						.Every(1.Seconds())
						.Using(message => { future.Complete(message.Count); });
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (UntypedChannelAdapter),
						typeof (UntypedChannelRouter),
						typeof (TypedChannelAdapter<TestMessage>),
						typeof (IntervalChannel<TestMessage>),
						typeof (ConsumerChannel<ICollection<TestMessage>>),
					});

				for (int i = 0; i < expected; i++)
				{
					input.Send(new TestMessage());
				}
			}

			future.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();
			future.Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_be_one_consumer_on_the_channel()
		{
			var input = new UntypedChannelAdapter(new SynchronousFiber());
			using (input.Subscribe(x =>
				{
					x.Consume<TestMessage>()
						.Using(message => { });
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (UntypedChannelAdapter),
						typeof (UntypedChannelRouter),
						typeof (TypedChannelAdapter<TestMessage>),
						typeof (ConsumerChannel<TestMessage>)
					});
			}
		}

		[Test]
		public void Should_be_two_consumers_on_the_channel()
		{
			var input = new UntypedChannelAdapter(new SynchronousFiber());
			using (input.Subscribe(x =>
				{
					x.Consume<TestMessage>()
						.Using(message => { });

					x.Consume<TestMessage>()
						.Using(message => { });
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (UntypedChannelAdapter),
						typeof (UntypedChannelRouter),
						typeof (TypedChannelAdapter<TestMessage>),
						typeof (ChannelRouter<TestMessage>),
						typeof (ConsumerChannel<TestMessage>),
						typeof (ConsumerChannel<TestMessage>),
					});
			}
		}
	}
}