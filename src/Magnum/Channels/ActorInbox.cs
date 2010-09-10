// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Channels
{
	using System;
	using Actors;
	using Collections;
	using Fibers;

	/// <summary>
	/// An inbox for an actor. Channel properties on the actor are automatically bound.
	/// Messages are automatically delivered to the inbox for each message type unless
	/// a property channel has the same message type. Calling Receive on the inbox will 
	/// </summary>
	/// <typeparam name="TActor">The actor type for this inbox</typeparam>
	public class ActorInbox<TActor> :
		Inbox
		where TActor : class, Actor
	{
		readonly Fiber _fiber;
		readonly Scheduler _scheduler;
		readonly Cache<Type, object> _inboxCache;
		readonly UntypedChannel _adapter;

		public ActorInbox(Fiber fiber, Scheduler scheduler, TActor instance)
			: this(fiber, scheduler)
		{
			_adapter.Connect(x =>
				{
					x.BindChannelsFor<TActor>()
						.UsingInstance(instance);
				});
		}

		public ActorInbox(Fiber fiber, Scheduler scheduler)
		{
			_fiber = fiber;
			_scheduler = scheduler;

			_adapter = new ChannelAdapter();

			_inboxCache = new Cache<Type, object>();
		}

		public void Send<T>(T message)
		{
			_fiber.Add(() =>
				{
					GetInbox<T>();
						
					_adapter.Send(message);
				});
		}

		public void Receive<T>(SelectiveConsumer<T> consumer)
		{
			_fiber.Add(() => GetInbox<T>().Receive(consumer));
		}

		public void Receive<T>(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback)
		{
			_fiber.Add(() => GetInbox<T>().Receive(consumer, timeout, timeoutCallback));
		}

		public void Receive<T>(SelectiveConsumer<T> consumer, int timeout, Action timeoutCallback)
		{
			_fiber.Add(() => GetInbox<T>().Receive(consumer, timeout, timeoutCallback));
		}

		Inbox<T> GetInbox<T>()
		{
			return _inboxCache.Retrieve(typeof(T), type =>
				{
					var inbox = new SynchronousInbox<T>(_fiber, _scheduler);

					_adapter.Connect(x => x.AddChannel(inbox));

					return inbox;

				}) as Inbox<T>;
		}
	}
}