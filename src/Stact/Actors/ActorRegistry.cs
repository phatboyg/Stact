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
	using System.Collections.Generic;
	
	using Configuration;

	/// <summary>
	/// An actor registry provide running storage for actors that are active in the system
	/// </summary>
	public interface ActorRegistry
	{
		/// <summary>
		/// Adds an actor to the registry
		/// </summary>
		/// <typeparam name="T">The type of the actor</typeparam>
		/// <param name="actor">The actor to add</param>
		void Register<T>(T actor)
			where T : Actor;

		/// <summary>
		/// Removes an actor from the registry
		/// </summary>
		/// <typeparam name="T">The type of the actor</typeparam>
		/// <param name="actor">The actor to remove</param>
		void Unregister<T>(T actor)
			where T : Actor;

		/// <summary>
		/// Removes an actor from the registry
		/// </summary>
		/// <param name="id">The id of the actor to remove</param>
		void Unregister(Guid id);

		/// <summary>
		/// Stops all actors and removes them from the registry
		/// </summary>
		void Shutdown();

		/// <summary>
		/// Gets an actor from the registry
		/// </summary>
		/// <param name="id">The id of the actor</param>
		/// <returns>The actor with the specified id</returns>
		Actor Get(Guid id);

		/// <summary>
		/// Gets all actors from the registry
		/// </summary>
		/// <returns>All actors in the registry</returns>
		IList<ActorInstance> GetAll();

		/// <summary>
		/// Get all actors from the registry that are the specified type
		/// </summary>
		/// <typeparam name="T">The type of actor to retrieve</typeparam>
		/// <returns>A list of actors matching the specified type</returns>
		IList<T> GetAll<T>()
			where T : Actor;

		/// <summary>
		/// Get all actors from the registry that are the specified type
		/// </summary>
		/// <param name="actorType">The type of actor to retrieve</param>
		/// <returns>A list of actors matching the specified type</returns>
		IEnumerable<Actor> GetAll(Type actorType);

		/// <summary>
		/// Calls the callback for each actor in the registry
		/// </summary>
		/// <param name="callback">A method to call with each actor</param>
		void Each(Action<Actor> callback);

		/// <summary>
		/// Calls the callback for each actor in the registry of the specified type
		/// </summary>
		/// <typeparam name="T">The type of actor to select</typeparam>
		/// <param name="callback">The method to call with each actor</param>
		void Each<T>(Action<T> callback)
			where T : Actor;

		/// <summary>
		/// Calls the callback for each actor in the registry of the specified type
		/// </summary>
		/// <param name="actorType">The type of actor to select</param>
		/// <param name="callback">The method to call with each actor</param>
		void Each(Type actorType, Action<Actor> callback);


		/// <summary>
		/// Allow subscription to events that are produced by the actor registry as actors
		/// are registered and unregistered.
		/// </summary>
		/// <param name="subscriberActions">The subscription actions</param>
		/// <returns>A channel subscription</returns>
		ChannelConnection Subscribe(Action<ConnectionConfigurator> subscriberActions);
		ChannelConnection Subscribe(Channel<ActorRegistered> listener);
		ChannelConnection Subscribe(Channel<ActorUnregistered> listener);
		ChannelConnection Subscribe(Channel<ActorRegistered> registeredListener, Channel<ActorUnregistered> unregisteredListener);
		void Add(Guid auctionId, ActorInstance auction);
	}

	public interface ActorRegistryEvent
	{
	}
    
	public interface ActorRegistered :
		ActorRegistryEvent
	{
		Actor Actor { get; }
	}

	public interface ActorUnregistered :
		ActorRegistryEvent
	{
		Actor Actor { get; }
	}
}