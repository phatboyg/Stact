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
namespace Stact.Actors
{
	using System;
	using Fibers;
	using Internal;

	/// <summary>
	/// Creates anonymous actor instances using the provided factory methods
	/// </summary>
	public class AnonymousActorFactory
	{
		readonly Func<Fiber, Scheduler, Inbox, AnonymousActor> _factory;
		readonly FiberFactory _fiberFactory;
		readonly SchedulerFactory _schedulerFactory;

		public AnonymousActorFactory(FiberFactory fiberFactory, SchedulerFactory schedulerFactory,
		                             Func<Fiber, Scheduler, Inbox, AnonymousActor> factory)
		{
			_fiberFactory = fiberFactory;
			_schedulerFactory = schedulerFactory;
			_factory = factory;
		}

		public ActorInstance Create(Action<Inbox> initializer)
		{
			Fiber fiber = _fiberFactory();
			Scheduler scheduler = _schedulerFactory();

			var inbox = new ActorInbox<AnonymousActor>(fiber, scheduler);

			_factory(fiber, scheduler, inbox);

			fiber.Add(() => initializer(inbox));

			return inbox;
		}
	}
}