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
namespace Stact
{
	using System;
	using Internal;
	using Magnum;


	/// <summary>
	/// An anonymous actor is used to declare behavior inline, rather than using a 
	/// class. 
	/// </summary>
	public class AnonymousActor :
		Actor
	{
		static AnonymousActorFactory _factory;
		static FiberFactory _fiberFactory;
		static SchedulerFactory _schedulerFactory;
		readonly Fiber _fiber;
		readonly Inbox _inbox;
		readonly Scheduler _scheduler;

		static AnonymousActor()
		{
			_fiberFactory = () => new PoolFiber();

			_schedulerFactory = () => new TimerScheduler(new PoolFiber());

			CreateAnonymousActorFactory();
		}

		AnonymousActor(Fiber fiber, Scheduler scheduler, Inbox inbox)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_inbox = inbox;
		}

		public static void SetAnonymousActorFiberFactory(FiberFactory fiberFactory)
		{
			Guard.AgainstNull(fiberFactory, "fiberFactory");
			_fiberFactory = fiberFactory;

			CreateAnonymousActorFactory();
		}

		public static void SetAnonymousActorSchedulerFactory(SchedulerFactory schedulerFactory)
		{
			Guard.AgainstNull(schedulerFactory, "schedulerFactory");

			_schedulerFactory = schedulerFactory;

			CreateAnonymousActorFactory();
		}

		static void CreateAnonymousActorFactory()
		{
			_factory = ActorFactory.CreateAnonymousActorFactory(x =>
				{
					x.ConstructedBy(CreateAnonymousActor);
					x.UseSchedulerFactory(_schedulerFactory);
					x.UseFiberFactory(_fiberFactory);
				});
		}

		static AnonymousActor CreateAnonymousActor(Fiber fiber, Scheduler scheduler, Inbox inbox)
		{
			return new AnonymousActor(fiber, scheduler, inbox);
		}

		public static ActorInstance New(Action<Inbox> initializer)
		{
			return _factory.GetActor(initializer);
		}
	}
}