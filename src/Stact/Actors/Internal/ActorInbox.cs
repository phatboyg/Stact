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
namespace Stact.Actors.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using Magnum.Collections;
	using Magnum.Extensions;
	using Stact.Internal;


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
		readonly Cache<Type, object> _inboxCache;
		IList<ChannelConnection> _connections;
		readonly Scheduler _scheduler;
		ChannelConnection _inboxConnection;

		public ActorInbox(Fiber fiber, Scheduler scheduler)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_connections = new List<ChannelConnection>();

			_adapter = new ChannelAdapter();
			_inboxConnection = _adapter.Connect(x =>
				{
					x.AddConsumerOf<Request<Exit>>()
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

		Inbox<T> GetInbox<T>()
		{
			return _inboxCache.Retrieve(typeof(T), type =>
				{
					var inbox = new BufferedInbox<T>(_fiber, _scheduler);

					ChannelConnection connection = _adapter.Connect(x => x.AddChannel(inbox));

					return inbox;
				}) as Inbox<T>;
		}

		public void BindChannelsForInstance(TActor actor)
		{
			_connections.Add(_adapter.Connect(x =>
				{
					x.BindChannelsFor<TActor>()
						.UsingInstance(actor)
						.HandleOnCallingThread();

					// actor channels are called from the calling thread since
					// the actor itself defines the calling style for the actual
					// channel - making this essentially a passthrough of Send()
					// to the actor channel implementation
				}));
		}

		void HandleExit(Request<Exit> message)
		{
			_connections.Each(x =>
				{
					x.Dispose();
				});
			_connections.Clear();

			_inboxCache.GetAll().Cast<IDisposable>().Each(x => x.Dispose());
			_inboxCache.ClearAll();

			_inboxConnection.Dispose();
			_inboxConnection = null;

			_fiber.Shutdown(TimeSpan.Zero);
		}

		void HandleKill(Kill message)
		{
			_fiber.Stop();
		}
	}
}