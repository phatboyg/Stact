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
	using Fibers;


	public class AnonymousActor :
		Actor
	{
		static readonly AnonymousActorFactory _factory;
		readonly Fiber _fiber;
		readonly Inbox _inbox;
		readonly Scheduler _scheduler;

		static AnonymousActor()
		{
			_factory = new AnonymousActorFactory(() => new ThreadPoolFiber(),
			                                     () => new TimerScheduler(new ThreadPoolFiber()),
			                                     (f, s, i) => new AnonymousActor(f, s, i));
		}

		AnonymousActor(Fiber fiber, Scheduler scheduler, Inbox inbox)
		{
			_fiber = fiber;
			_scheduler = scheduler;
			_inbox = inbox;
		}

		public static ActorInstance New(Action<Inbox> initializer)
		{
			return _factory.Create(initializer);
		}
	}
}