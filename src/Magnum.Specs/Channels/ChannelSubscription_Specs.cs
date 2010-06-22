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

			var input = new ChannelAdapter();
			int expected = 5;
			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.Every(1.Seconds(), c => c.Value)
						.UsingConsumer(message => future.Complete(message.Count));
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (ChannelAdapter),
						typeof (BroadcastChannel),
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
			var input = new ChannelAdapter();
			using (input.Connect(x => { }))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (ChannelAdapter),
						typeof (ShuntChannel)
					});
			}
		}

		[Test]
		public void Should_add_an_untyped_channel_to_an_untyped_channel_adapter()
		{
			var next = new ChannelAdapter();

			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddUntypedChannel(next);
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (ChannelAdapter),
						typeof (BroadcastChannel),
						typeof(ChannelAdapter),
						typeof(ShuntChannel),
					});
			}
		}

		[Test]
		public void Should_be_an_interval_consumer_on_the_channel()
		{
			var future = new Future<int>();

			var input = new ChannelAdapter();
			int expected = 5;
			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.Every(1.Seconds())
						.UsingConsumer(message => future.Complete(message.Count));
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (ChannelAdapter),
						typeof (BroadcastChannel),
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
			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.UsingConsumer(message => { });
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (ChannelAdapter),
						typeof (BroadcastChannel),
						typeof (TypedChannelAdapter<TestMessage>),
						typeof (ConsumerChannel<TestMessage>)
					});
			}
		}

		[Test]
		public void Should_be_two_consumers_on_the_channel()
		{
			SelectiveConsumer<TestMessage> selectiveConsumer = x => y => { };

			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.UsingConsumer(message => { });

					x.AddConsumerOf<TestMessage>()
						.UsingSelectiveConsumer(selectiveConsumer);
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof (ChannelAdapter),
						typeof (BroadcastChannel),
						typeof (TypedChannelAdapter<TestMessage>),
						typeof (BroadcastChannel<TestMessage>),
						typeof (ConsumerChannel<TestMessage>),
						typeof (SelectiveConsumerChannel<TestMessage>),
					});
			}
		}
	}
}