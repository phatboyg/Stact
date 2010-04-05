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
	using Magnum.Actions;
	using Magnum.Actors;
	using Magnum.Channels;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Binding_an_action_queue_channel_to_a_method
	{
		[Test]
		public void Should_call_the_consumer_method()
		{
			var actor = new SomeActorInstance();

			var message = new MyMessage();

			actor.MessageChannel.Send(message);

			actor.Future.IsAvailable(1.Seconds()).ShouldBeTrue();
			actor.Future.Value.ShouldEqual(message);
		}

		[Test]
		public void Should_call_the_consumer_anonymous_method()
		{
			var actor = new SomeActorInstance();

			var message = new MyMessage();

			actor.LambdaMessageChannel.Send(message);

			actor.Future.IsAvailable(1.Seconds()).ShouldBeTrue();
			actor.Future.Value.ShouldEqual(message);
		}
	}


	public class SomeActorInstance
	{
		private readonly Future<MyMessage> _future;
		private readonly ThreadPoolActionQueue _queue;

		public SomeActorInstance()
		{
			_queue = new ThreadPoolActionQueue();
			_future = new Future<MyMessage>();

			MessageChannel = new ConsumerChannel<MyMessage>(_queue, Consume);
			LambdaMessageChannel = new ConsumerChannel<MyMessage>(_queue, message => _future.Complete(message));
		}

		public Future<MyMessage> Future
		{
			get { return _future; }
		}

		public Channel<MyMessage> MessageChannel { get; private set; }
		public Channel<MyMessage> LambdaMessageChannel { get; private set; }

		private void Consume(MyMessage message)
		{
			_future.Complete(message);
		}
	}

	public class MyMessage
	{
		public Guid Id { get; set; }
	}
}