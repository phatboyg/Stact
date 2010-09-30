// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Sample.Actors.Console
{
	using Stact.Actors;
	using Stact.Fibers;
	using StructureMap;

	public class Bootstrapper
	{
		public void Bootstrap()
		{
			ObjectFactory.Initialize(x =>
				{
					x.For<Fiber>().AlwaysUnique().Use<ThreadPoolFiber>();
					x.SelectConstructor(() => new ThreadPoolFiber());
					
					x.For<Scheduler>().Singleton().Use<TimerScheduler>();

					x.For(typeof (ActorCache<,>)).Singleton().Use(typeof (ActorCache<,>));

					x.For<ActorProvider<Seller, string>>().Use(
						context => key => context.GetInstance<ActorCache<Seller, string>>().Get(key));
				});
		}
	}
}