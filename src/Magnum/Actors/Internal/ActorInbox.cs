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
namespace Magnum.Actors.Internal
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using Channels;
	using Collections;
	using Extensions;
	using Fibers;
	using Messages;


	/// <summary>
	/// An inbox for an actor. Channel properties on the actor are automatically bound.
	/// Messages are automatically delivered to the inbox for each message type unless
	/// a property channel has the same message type. Calling Receive on the inbox will 
	/// </summary>
	/// <typeparam name="TActor">The actor type for this inbox</typeparam>
	public class ActorInbox<TActor> :
		ActorInstance,
		Inbox
		where TActor : class, Actor
	{
		readonly UntypedChannel _adapter;
		readonly Fiber _fiber;
		readonly object _iFuckingHateThisShit = new object();
		readonly Cache<Type, object> _inboxCache;
		readonly Scheduler _scheduler;
		ChannelConnection _actorConnection;
		ChannelConnection _internalConnection;

		public ActorInbox(Fiber fiber, Scheduler scheduler)
		{
			_fiber = fiber;
			_scheduler = scheduler;

			_adapter = new ChannelAdapter();
			_internalConnection = _adapter.Connect(x =>
				{
					x.AddConsumerOf<Exit>()
						.UsingConsumer(HandleExit)
						.HandleOnFiber(_fiber);
					x.AddConsumerOf<Kill>()
						.UsingConsumer(HandleKill)
						.HandleOnCallingThread();
				});

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

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer)
		{
			return GetInbox<T>().Receive(consumer);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback)
		{
			return GetInbox<T>().Receive(consumer, timeout, timeoutCallback);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer, int timeout, Action timeoutCallback)
		{
			return GetInbox<T>().Receive(consumer, timeout, timeoutCallback);
		}

		Inbox<T> GetInbox<T>()
		{
			lock (_iFuckingHateThisShit)
				return _inboxCache.Retrieve(typeof(T), type =>
					{
						var inbox = new BufferedInbox<T>(_fiber, _scheduler);

						_adapter.Connect(x => x.AddChannel(inbox));

						return inbox;
					}) as Inbox<T>;
		}

		public void BindChannelsForInstance(TActor actor)
		{
			_actorConnection = _adapter.Connect(x =>
				{
					x.BindChannelsFor<TActor>()
						.UsingInstance(actor)
						.HandleOnCallingThread();

					// actor channels are called from the calling thread since
					// the actor itself defines the calling style for the actual
					// channel - making this essentially a passthrough of Send()
					// to the actor channel implementation
				});
		}

		void HandleExit(Exit message)
		{
			if (_actorConnection != null)
			{
				_actorConnection.Dispose();
				_actorConnection = null;
			}

			_inboxCache.GetAll().Cast<IDisposable>().Each(x => x.Dispose());
			_inboxCache.ClearAll();

			_internalConnection.Dispose();
			_internalConnection = null;
		}

		void HandleKill(Kill message)
		{
			_fiber.Stop();
		}
	}
}