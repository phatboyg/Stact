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
	using System.Diagnostics;
	
	using Internal;
	using Stact;
	using Stact.Visitors;
	using NUnit.Framework;
	using Magnum.TestFramework;

	[TestFixture]
	public class Subscribing_to_an_untyped_channel_adapter
	{
		private class TestConsumer
		{
			public TestConsumer()
			{
				InputChannel = new ConsumerChannel<TestMessage>(new SynchronousFiber(), x => Future.Complete(x));
			}

			public static Future<TestMessage> Future { get; set; }
			public Channel<TestMessage> InputChannel { get; private set; }
		}

		private class TestMessage
		{
		}

		[Test]
		public void Should_add_a_provider_based_consumer()
		{
			var input = new ChannelAdapter();

			var futureA = new Future<TestMessage>();
			var futureB = new Future<TestMessage>();
			var futureC = new Future<TestMessage>();

			TestConsumer.Future = futureA;

			using (input.Connect(x =>
				{
					x.AddConsumerOf<TestMessage>()
						.UsingInstance().Of<TestConsumer>()
						.ObtainedBy(() => new TestConsumer())
						.OnChannel(y => y.InputChannel);

					x.AddConsumerOf<TestMessage>()
						.UsingConsumer(futureB.Complete);

					x.AddConsumerOf<TestMessage>()
						.UsingSelectiveConsumer(message => futureC.Complete);
				}))
			{
				Trace.WriteLine("Complete network:");
				new TraceChannelVisitor().Visit(input);

				input.Send(new TestMessage());
			}

			Trace.WriteLine("Empty network:");
			new TraceChannelVisitor().Visit(input);

			futureA.IsCompleted.ShouldBeTrue();
			futureB.IsCompleted.ShouldBeTrue();
			futureC.IsCompleted.ShouldBeTrue();
		}


		[Test]
		public void Should_register_my_consumer()
		{
			var input = new ChannelAdapter();

			var futureA = new Future<TestMessage>();
			var consumerA = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureA.Complete);

			var futureB = new Future<TestMessage>();
			var consumerB = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureB.Complete);

			using (input.Connect(x =>
				{
					x.AddChannel(consumerA);
					x.AddChannel(consumerB);
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
			var input = new ChannelAdapter();

			var futureA = new Future<TestMessage>();
			var consumerA = new ConsumerChannel<TestMessage>(new SynchronousFiber(), futureA.Complete);

			using (ChannelConnection connection = input.Connect(x => x.AddChannel(consumerA)))
			{
			}

			new TraceChannelVisitor().Visit(input);

			input.Send(new TestMessage());

			futureA.IsCompleted.ShouldBeFalse();
		}
	}
}