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
namespace Magnum.Actors
{
	using System;
	using Channels;
	using Fibers;
	using Internal;


	public class AnonymousActorFactory
	{
		readonly Func<Fiber, Scheduler, Inbox, AnonymousActor> _factory;
		readonly Func<Fiber> _fiberFactory;
		readonly Func<Scheduler> _schedulerFactory;

		public AnonymousActorFactory(Func<Fiber> fiberFactory, Func<Scheduler> schedulerFactory,
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

			initializer(inbox);

			_factory(fiber, scheduler, inbox);

			return inbox;
		}
	}
}