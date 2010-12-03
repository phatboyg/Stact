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
namespace Stact.Actors.Internal
{
	using System;
	using System.Collections.Generic;
	using Magnum.Collections;
	using Routing;
	using Routing.Internal;
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
		readonly Fiber _fiber;
		readonly Scheduler _scheduler;
		UntypedChannel _engine;

		Cache<Type, object> _joinNodes;
		HashSet<PendingReceive> _pending;


		public ActorInbox(Fiber fiber, Scheduler scheduler)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_engine = new DynamicRoutingEngine(fiber);

			_joinNodes = new Cache<Type, object>();

			_pending = new HashSet<PendingReceive>();
			_pending.Add(Receive<Request<Exit>>(x => HandleExit));
			_pending.Add(Receive<Kill>(x => HandleKill));
		}

		public void Send<T>(T message)
		{
			_engine.Send(message);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer)
		{
			var pending = new PendingReceiveImpl<T>(this, consumer, x => _pending.Remove(x));

			return Receive(pending);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback)
		{
			var pending = new PendingReceiveImpl<T>(this, consumer, timeoutCallback, x => _pending.Remove(x));

			pending.ScheduleTimeout(x => _scheduler.Schedule(timeout, _fiber, x.Timeout));

			return Receive(pending);
		}

		void HandleExit(Request<Exit> message)
		{
			_fiber.Shutdown(TimeSpan.Zero);
		}

		void HandleKill(Kill message)
		{
			_fiber.Stop();
		}

		PendingReceive Receive<T>(PendingReceiveImpl<T> receiver)
		{
			var consumerNode = new SelectiveConsumerNode<T>(receiver.Accept);

			var joinNode = (JoinNode<T>)_joinNodes.Retrieve(typeof(T), x =>
				{
					JoinNode<T> result = null;
					var locator = new JoinNodeLocator<T>(jNode => result = jNode);
					locator.Search(_engine);
					return result;
				});

			joinNode.AddActivation(consumerNode);

			_pending.Add(receiver);
			return receiver;
		}
	}
}