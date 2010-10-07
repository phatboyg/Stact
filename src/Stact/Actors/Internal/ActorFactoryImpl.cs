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
namespace Stact.Internal
{
	using System;
	using Actors.Internal;
	


	public class ActorFactoryImpl<TActor> :
		ActorFactory<TActor>,
		AnonymousActorFactory
		where TActor : class, Actor
	{
		readonly Func<Fiber, Scheduler, Inbox, TActor> _factory;
		readonly FiberFactory _fiberFactory;
		readonly SchedulerFactory _schedulerFactory;

		public ActorFactoryImpl(FiberFactory fiberFactory, SchedulerFactory schedulerFactory,
		                        Func<Fiber, Scheduler, Inbox, TActor> factory)
		{
			_fiberFactory = fiberFactory;
			_schedulerFactory = schedulerFactory;
			_factory = factory;
		}

		public ActorInstance GetActor()
		{
			return GetActor(null);
		}

		public ActorInstance GetActor(Action<Inbox> initializer)
		{
			Fiber fiber = _fiberFactory();
			Scheduler scheduler = _schedulerFactory();

			var inbox = new ActorInbox<TActor>(fiber, scheduler);

			TActor actor = CreateActorInstance(fiber, scheduler, inbox);

			inbox.BindChannelsForInstance(actor);

			if (initializer != null)
				fiber.Add(() => initializer(inbox));

			return inbox;
		}

		TActor CreateActorInstance(Fiber fiber, Scheduler scheduler, Inbox inbox)
		{
			return _factory(fiber, scheduler, inbox);
		}
	}
}