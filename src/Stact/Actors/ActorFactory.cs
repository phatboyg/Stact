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
	using Actors;


	public static class ActorFactory
	{
		public static ActorFactory<TActor> Create<TActor>(Action<ActorFactoryConfigurator<TActor>> configurator) 
			where TActor : class, Actor
		{
			var factoryConfiguratorImpl = new ActorFactoryConfiguratorImpl<TActor>();

			configurator(factoryConfiguratorImpl);
			
			return factoryConfiguratorImpl.CreateActorFactory();
		}
	}

	/// <summary>
	/// A builder abstraction for creating actor instances when needed
	/// </summary>
	/// <typeparam name="TActor">The actor type</typeparam>
	public interface ActorFactory<TActor>
		where TActor : class, Actor
	{
		/// <summary>
		/// Returns an instance of an actor
		/// </summary>
		/// <returns></returns>
		ActorInstance GetActor();
	}
}