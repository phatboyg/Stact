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
	using Configuration;
	using Events;


	/// <summary>
	/// An actor registry provide running storage for actors that are active in the system
	/// </summary>
	public interface ActorRegistry
	{
		/// <summary>
		/// Adds an actor instance to the registry
		/// </summary>
		/// <param name="key">The unique identifier for the actor instance</param>
		/// <param name="actor">The actor instance</param>
		void Register(Guid key, ActorInstance actor);

		/// <summary>
		/// Adds an actor to the registry
		/// </summary>
		/// <param name="actor">The actor to add</param>
		/// <param name="callback"></param>
		void Register(ActorInstance actor, Action<Guid, ActorInstance> callback);

		/// <summary>
		/// Removes an actor from the registry
		/// </summary>
		/// <typeparam name="T">The type of the actor</typeparam>
		/// <param name="actor">The actor to remove</param>
		void Unregister(ActorInstance actor);

		/// <summary>
		/// Removes an actor from the registry
		/// </summary>
		/// <param name="key">The id of the actor to remove</param>
		void Unregister(Guid key);

		/// <summary>
		/// Stops all actors and removes them from the registry
		/// </summary>
		void Shutdown();

		/// <summary>
		/// Gets an actor from the registry
		/// </summary>
		/// <param name="key">The id of the actor</param>
		/// <param name="callback"></param>
		/// <param name="notFoundCallback"></param>
		void Get(Guid key, Action<ActorInstance> callback, Action notFoundCallback);

		/// <summary>
		/// Calls the callback for each actor in the registry
		/// </summary>
		/// <param name="callback">A method to call with each actor</param>
		void Each(Action<Guid, ActorInstance> callback);

		/// <summary>
		/// Allow subscription to events that are produced by the actor registry as actors
		/// are registered and unregistered.
		/// </summary>
		/// <param name="subscriberActions">The subscription actions</param>
		/// <returns>A channel subscription</returns>
		ChannelConnection Subscribe(Action<ConnectionConfigurator> subscriberActions);

		ChannelConnection Subscribe(Channel<ActorRegistered> listener);
		ChannelConnection Subscribe(Channel<ActorUnregistered> listener);

		ChannelConnection Subscribe(Channel<ActorRegistered> registeredListener,
		                            Channel<ActorUnregistered> unregisteredListener);
	}
}