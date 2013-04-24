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
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[TestFixture]
	public class Building_an_aggregator_network
	{
		[SetUp]
		public void Setup()
		{
			_fiber = new SynchronousFiber();
			_timeout = 100.Milliseconds();
		}

		[Test]
		public void Should_add_a_consumer_that_is_assignable_to_the_type()
		{
			var firstFuture = new Future<TestMessage>();
			var secondFuture = new Future<ITestMessage>();

			var first = new ConsumerChannel<TestMessage>(_fiber, firstFuture.Complete);
			var subs = new BroadcastChannel<TestMessage>(new[] {first});
			var adapter = new ChannelAdapter<TestMessage>(subs);

			var second = new ConsumerChannel<ITestMessage>(_fiber, secondFuture.Complete);

			using (ChannelConnection scope = adapter.Connect(x => x.AddChannel(second)))
			{
	//			new TraceChannelVisitor().Visit(adapter);

				adapter.Send(new TestMessage());
			}

			firstFuture.IsCompleted.ShouldBeTrue();
			secondFuture.IsCompleted.ShouldBeTrue();
		}


		[Test]
		public void Should_add_a_consumer_to_an_empty_adapter_chain()
		{
			var adapter = new ChannelAdapter<TestMessage>(new ShuntChannel<TestMessage>());

			var future = new Future<TestMessage>();
			var consumer = new ConsumerChannel<TestMessage>(_fiber, future.Complete);

			using (ChannelConnection scope = adapter.Connect(x =>
			{
				x.AddChannel(consumer);
			}))
			{
		//		new TraceChannelVisitor().Visit(adapter);

				adapter.Send(new TestMessage());
			}

			future.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_add_a_consumer_to_an_existing_adapter_chain()
		{
			var firstFuture = new Future<TestMessage>();
			var secondFuture = new Future<TestMessage>();

			var first = new ConsumerChannel<TestMessage>(_fiber, firstFuture.Complete);
			var subs = new BroadcastChannel<TestMessage>(new[] {first});
			var adapter = new ChannelAdapter<TestMessage>(subs);

			var second = new ConsumerChannel<TestMessage>(_fiber, secondFuture.Complete);

			using (ChannelConnection scope = adapter.Connect(x => x.AddChannel(second)))
			{
			//	new TraceChannelVisitor().Visit(adapter);

				adapter.Send(new TestMessage());
			}

			firstFuture.IsCompleted.ShouldBeTrue();
			secondFuture.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_manage_interleaved_changes_the_the_chain()
		{
			var firstFuture = new Future<TestMessage>();
			var secondFuture = new Future<TestMessage>();

			var adapter = new ChannelAdapter<TestMessage>(new ShuntChannel<TestMessage>());

			var first = new ConsumerChannel<TestMessage>(_fiber, firstFuture.Complete);
			ChannelConnection firstScope = adapter.Connect(x => x.AddChannel(first));

			var second = new ConsumerChannel<TestMessage>(_fiber, secondFuture.Complete);
			ChannelConnection secondScope = adapter.Connect(x => x.AddChannel(second));

			firstScope.Dispose();

			//new TraceChannelVisitor().Visit(adapter);

			adapter.Send(new TestMessage());

			firstFuture.IsCompleted.ShouldBeFalse();
			secondFuture.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_remove_a_consumer_from_an_adapter_chain()
		{
			var adapter = new ChannelAdapter<TestMessage>(new ShuntChannel<TestMessage>());

			var future = new Future<TestMessage>();

			var consumer = new ConsumerChannel<TestMessage>(_fiber, future.Complete);
			using (ChannelConnection scope = adapter.Connect(x => x.AddChannel(consumer))) {}

			//new TraceChannelVisitor().Visit(adapter);

			adapter.Send(new TestMessage());

			future.IsCompleted.ShouldBeFalse();
		}

		[Test]
		public void Should_send_to_a_adapter_consumer_chain()
		{
			var future = new Future<TestMessage>();

			var consumer = new ConsumerChannel<TestMessage>(_fiber, future.Complete);
			var adapter = new ChannelAdapter<TestMessage>(consumer);

			adapter.Send(new TestMessage());

			future.IsCompleted.ShouldBeTrue();
		}

		SynchronousFiber _fiber;
		TimeSpan _timeout;

		[TestFixtureSetUp]
		public void SetupAll() {}


		interface ITestMessage {}


		class TestMessage :
			ITestMessage {}
	}
}