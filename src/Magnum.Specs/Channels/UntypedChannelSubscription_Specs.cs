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
	using Fibers;
	using Magnum.Channels;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Subscribing_to_an_untyped_channel_adapter
	{
		private class TestConsumer
		{
			public Channel<TestMessage> InputChannel { get; private set; }
		}

		private class TestMessage
		{
		}

		[Test, Explicit]
		public void Should_add_a_provider_based_consumer()
		{
			var input = new UntypedChannelAdapter(new SynchronousFiber());

			var futureA = new Future<TestMessage>();

			using (ChannelSubscription subscription = input.Subscribe(x =>
				{
					// TODO move this up to a nested closure inside the Subscribe() method to build the entries 
					// and then execute them before returning the disposable subscription info
					x.Consume<TestMessage>()
						.Using<TestConsumer>(y => y.InputChannel)
						.ObtainedBy(message => new TestConsumer());

					x.Consume<TestMessage>()
						.Using(message => { });

					x.Consume<TestMessage>()
						.Using(message => { return m => { }; });
				}))
			{
			}

			new TraceChannelVisitor().Visit(input);

			input.Send(new TestMessage());

			futureA.IsCompleted.ShouldBeFalse();
		}

		[Test]
		public void Should_register_my_consumer()
		{
			var input = new UntypedChannelAdapter(new SynchronousFiber());

			var futureA = new Future<TestMessage>();
			var consumerA = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureA.Complete);

			var futureB = new Future<TestMessage>();
			var consumerB = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureB.Complete);

			using (ChannelSubscription subscription = input.Subscribe(x =>
				{
					x.Add(consumerA);
					x.Add(consumerB);
				}))
			{
				new TraceChannelVisitor().Visit(input);

				input.Send(new TestMessage());
			}

			futureA.IsCompleted.ShouldBeTrue();
			futureB.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_remove_my_consumer()
		{
			var input = new UntypedChannelAdapter(new SynchronousFiber());

			var futureA = new Future<TestMessage>();
			var consumerA = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureA.Complete);

			using (ChannelSubscription subscription = input.Subscribe(x => x.Add(consumerA)))
			{
			}

			new TraceChannelVisitor().Visit(input);

			input.Send(new TestMessage());

			futureA.IsCompleted.ShouldBeFalse();
		}
	}
}