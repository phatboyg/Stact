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
	using Collections;
	using Magnum.Actions;
	using Magnum.Channels;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Sending_a_message_to_a_consumer_pool
	{
		private class MyCommand
		{
			public Guid Id { get; set; }
		}

		private class MyConsumer
		{
			private readonly ActionQueue _queue;

			public MyConsumer(ActionQueue queue)
			{
				_queue = queue;

				Called = new Future<MyCommand>();
				CommandChannel = new ConsumerChannel<MyCommand>(_queue, HandleMyCommand);
			}

			public Future<MyCommand> Called { get; private set; }

			public Channel<MyCommand> CommandChannel { get; private set; }

			private void HandleMyCommand(MyCommand message)
			{
				Called.Complete(message);
			}
		}

		[Test]
		public void Should_be_able_to_call_the_consumer_directly()
		{
			var consumer = new MyConsumer(new SynchronousActionQueue());

			consumer.CommandChannel.Send(new MyCommand());

			consumer.Called.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_get_the_key_from_the_message()
		{
			KeyAccessor<MyCommand, Guid> getKey = message => message.Id;

			Guid id = CombGuid.Generate();
			var command = new MyCommand {Id = id};

			Guid key = getKey(command);

			key.ShouldEqual(id);
		}

		[Test]
		public void Should_properly_dispatch_the_message_to_an_instance()
		{
			Guid id = CombGuid.Generate();

			var command = new MyCommand {Id = id};

		}
	}


	public class ConsumerPool<TConsumer, TMessage, TKey>
	{
		private readonly IConsumerDictionary<TKey, TConsumer> _dictionary;
		private readonly ChannelAccessor<TConsumer, TMessage> _getChannel;
		private readonly KeyAccessor<TMessage, TKey> _getKey;
		private readonly ActionQueue _queue;

		public ConsumerPool(ActionQueue queue, IConsumerDictionary<TKey, TConsumer> dictionary, KeyAccessor<TMessage, TKey> getKey, ChannelAccessor<TConsumer, TMessage> getChannel)
		{
			_queue = queue;
			_dictionary = dictionary;
			_getKey = getKey;
			_getChannel = getChannel;

			InputChannel = new ConsumerChannel<TMessage>(queue, Dispatch);
		}

		public Channel<TMessage> InputChannel { get; private set; }

		private void Dispatch(TMessage message)
		{
			TKey key = _getKey(message);

			TConsumer consumer = _dictionary.Retrieve(key);

			Channel<TMessage> channel = _getChannel(consumer);

			channel.Send(message);
		}
	}


	public interface ConsumerChannelPolicy<TConsumer, TMessage>
	{
	}

	public interface ConsumerRepository<TKey, TConsumer>
	{
	}


	public class ConsumerDictionary<TKey, T> :
		IConsumerDictionary<TKey, T>
	{
		private readonly Cache<TKey, T> _cache;

		public ConsumerDictionary(Func<TKey, T> getConsumer)
		{
			_cache = new Cache<TKey, T>(getConsumer);
		}

		public T Retrieve(TKey key)
		{
			return _cache.Retrieve(key);
		}
	}

	public interface IConsumerDictionary<TKey, T>
	{
		T Retrieve(TKey key);
	}
}