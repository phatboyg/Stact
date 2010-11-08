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
namespace Stact.Specs.Registries
{
	using System;
	using System.Collections.Generic;
	using Configuration;
	using Internal;
	using Magnum;
	using Magnum.TestFramework;
	using Model;
	using NUnit.Framework;
	using Stact.Actors;


	[Scenario]
	public class When_adding_an_actor_to_a_registry
	{
		ActorFactory<Auction> _auctionFactory;
		Guid _auctionId;
		ActorRegistry _registry;

		public void Adding_an_actor_to_a_registry()
		{
			_auctionFactory = ActorFactory.Create(inbox => new Auction(inbox));

			_auctionId = CombGuid.Generate();

			ActorInstance auction = _auctionFactory.GetActor();

			ActorRegistry registry = new InMemoryActorRegistry(new PoolFiber());
			registry.Register(auction, (key, actor) => { });

			// need to proxy the channel with headers somehow... 

			// untyped channel => channel mapper -> actor instance

			// DestinationAddress -> set by outbound channel proxy on message<>
			// SourceAddress -> set by outbound channel proxy when available (not likely)
			// ResponseAddress -> set by outbound channel for ResponseChannel on Request to map to channel
			// Id -> system assigned id
			// DestinationAddress = urn:actor:554FC958-4661-4FE9-94F5-21D190417BCC
		}
	}


	public class InMemoryActorRegistry :
		ActorRegistry
	{
		readonly IDictionary<ActorInstance, Guid> _actors;

		readonly UntypedChannel _events;
		readonly Fiber _fiber;
		readonly IDictionary<Guid, ActorInstance> _keyIndex;

		public InMemoryActorRegistry(Fiber fiber)
		{
			_fiber = fiber;

			_actors = new Dictionary<ActorInstance, Guid>();
			_keyIndex = new Dictionary<Guid, ActorInstance>();

			_events = new ChannelAdapter();
		}

		public void Register(Guid key, ActorInstance actor)
		{
			_fiber.Add(() =>
				{
					ActorInstance existingActor;
					if (_keyIndex.TryGetValue(key, out existingActor))
					{
						if (ReferenceEquals(existingActor, actor))
							return;

						Remove(key, existingActor);
					}

					if (_actors.ContainsKey(actor))
					{
						_events.Send(new ActorUnregisteredImpl(_actors[actor], actor));

						_actors[actor] = key;
						_keyIndex[key] = actor;

						_events.Send(new ActorRegisteredImpl(_actors[actor], actor));
						return;
					}

					Add(key, actor);
				});
		}

		public void Register(ActorInstance actor, Action<Guid, ActorInstance> callback)
		{
			_fiber.Add(() =>
				{
					Guid key = CombGuid.Generate();
					Add(key, actor);

					callback(key, actor);
				});
		}

		public void Unregister(ActorInstance actor)
		{
			_fiber.Add(() =>
				{
					Guid key;
					if (_actors.TryGetValue(actor, out key))
						Remove(key, actor);
				});
		}

		public void Unregister(Guid key)
		{
			_fiber.Add(() =>
				{
					ActorInstance actor;
					if (_keyIndex.TryGetValue(key, out actor))
						Remove(key, actor);
				});
		}

		public void Shutdown()
		{
			_fiber.Add(() =>
				{
					foreach (ActorInstance actor in _actors.Keys)
						actor.Send<Exit>();
				});
		}

		public void Get(Guid key, Action<ActorInstance> callback, Action notFoundCallback)
		{
			_fiber.Add(() =>
				{
					ActorInstance actor;
					if (_keyIndex.TryGetValue(key, out actor))
						callback(actor);
					else
						notFoundCallback();
				});
		}

		public void Each(Action<Guid, ActorInstance> callback)
		{
			_fiber.Add(() =>
				{
					foreach (var pair in _keyIndex)
						callback(pair.Key, pair.Value);
				});
		}

		public ChannelConnection Subscribe(Action<ConnectionConfigurator> subscriberActions)
		{
			return _events.Connect(subscriberActions);
		}

		public ChannelConnection Subscribe(Channel<ActorRegistered> listener)
		{
			return _events.Connect(x => x.AddChannel(listener));
		}

		public ChannelConnection Subscribe(Channel<ActorUnregistered> listener)
		{
			return _events.Connect(x => x.AddChannel(listener));
		}

		public ChannelConnection Subscribe(Channel<ActorRegistered> registeredListener,
		                                   Channel<ActorUnregistered> unregisteredListener)
		{
			return _events.Connect(x =>
				{
					x.AddChannel(registeredListener);
					x.AddChannel(unregisteredListener);
				});
		}

		void Add(Guid key, ActorInstance actor)
		{
			_keyIndex.Add(key, actor);
			_actors.Add(actor, key);

			_events.Send(new ActorRegisteredImpl(key, actor));
		}

		void Remove(Guid key, ActorInstance actor)
		{
			_keyIndex.Remove(key);
			_actors.Remove(actor);

			_events.Send(new ActorUnregisteredImpl(key, actor));
		}
	}


	class ActorUnregisteredImpl :
		ActorUnregistered
	{
		public ActorUnregisteredImpl(Guid key, ActorInstance actor)
		{
			Key = key;
			Actor = actor;
		}

		public Guid Key { get; private set; }
		public ActorInstance Actor { get; private set; }
	}


	class ActorRegisteredImpl :
		ActorRegistered
	{
		public ActorRegisteredImpl(Guid key, ActorInstance actor)
		{
			Key = key;
			Actor = actor;
		}

		public Guid Key { get; private set; }
		public ActorInstance Actor { get; private set; }
	}
}