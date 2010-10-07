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
	
	using Internal;
	using Magnum;
	using Stact.Channels;
	using NUnit.Framework;
	using Magnum.TestFramework;


	[TestFixture]
	public class Sending_a_message_to_a_consumer_pool
	{
		[Test]
		public void Should_be_able_to_call_the_consumer_directly()
		{
			var consumer = new MyConsumer(new SynchronousFiber());

			consumer.CommandChannel.Send(new MyCommand());

			consumer.Called.IsCompleted.ShouldBeTrue();
		}

		[Test]
		public void Should_get_the_key_from_the_message()
		{
			KeyAccessor<MyCommand, Guid> getKey = message => message.Id;

			Guid id = CombGuid.Generate();
			var command = new MyCommand
				{
					Id = id
				};

			Guid key = getKey(command);

			key.ShouldEqual(id);
		}

		[Test]
		public void Should_properly_dispatch_the_message_to_an_instance()
		{
			Guid id = CombGuid.Generate();

			var command = new MyCommand
				{
					Id = id
				};
		}


		class MyCommand
		{
			public Guid Id { get; set; }
		}


		class MyConsumer
		{
			readonly Fiber _fiber;

			public MyConsumer(Fiber fiber)
			{
				_fiber = fiber;

				Called = new Stact.Future<MyCommand>();
				CommandChannel = new ConsumerChannel<MyCommand>(_fiber, HandleMyCommand);
			}

			public Stact.Future<MyCommand> Called { get; private set; }

			public Channel<MyCommand> CommandChannel { get; private set; }

			void HandleMyCommand(MyCommand message)
			{
				Called.Complete(message);
			}
		}
	}
}