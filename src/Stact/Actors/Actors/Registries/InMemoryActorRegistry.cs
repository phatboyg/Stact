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
namespace Stact.Actors.Registries
{
	using System;
	using System.Collections.Generic;
	using Configuration;
	using Events;
	using Events.Impl;
	using Magnum;
	using Magnum.Extensions;
	using Remote;


	public class InMemoryActorRegistry :
		ActorRegistry
	{
		readonly IDictionary<ActorRef, Guid> _actors;

		readonly UntypedChannel _events;
		readonly Fiber _fiber;
		readonly IDictionary<Guid, ActorRef> _keyIndex;
		UntypedChannel _channel;
		IList<RegistryNode> _nodes;

		public InMemoryActorRegistry(Fiber fiber)
		{
			_fiber = fiber;

			_actors = new Dictionary<ActorRef, Guid>();
			_keyIndex = new Dictionary<Guid, ActorRef>();

			_events = new ChannelAdapter();
			_channel = new ActorRegistryHeaderChannel(this);
			_nodes = new List<RegistryNode>();
		}

		public void Register(Guid key, ActorRef actor)
		{
			_fiber.Add(() =>
			{
				ActorRef existingActor;
				if (_keyIndex.TryGetValue(key, out existingActor))
				{
					if (ReferenceEquals(existingActor, actor))
						return;

					Remove(key, existingActor);
				}

				if (_actors.ContainsKey(actor))
				{
					if (existingActor != actor)
						_events.Send(new ActorUnregisteredImpl(this, actor, _actors[actor]));

					_actors[actor] = key;
					_keyIndex[key] = actor;

					_events.Send(new ActorRegisteredImpl(this, actor, key));
					return;
				}

				Add(key, actor);
			});
		}

		public void Register(ActorRef actor, Action<Guid, ActorRef> callback)
		{
			_fiber.Add(() =>
			{
				Guid key = CombGuid.Generate();
				Add(key, actor);

				callback(key, actor);
			});
		}

		public void Unregister(ActorRef actor)
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
				ActorRef actor;
				if (_keyIndex.TryGetValue(key, out actor))
					Remove(key, actor);
			});
		}

		public void Shutdown()
		{
			_fiber.Add(() =>
			{
				foreach (ActorRef actor in _actors.Keys)
					actor.Send<Exit>();

				foreach (RegistryNode node in _nodes)
				{
					node.Dispose();
				}
			});

			_fiber.Stop(3.Minutes());
		}

		public void Get(Guid key, Action<ActorRef> callback, Action notFoundCallback)
		{
			_fiber.Add(() =>
			{
				ActorRef actor;
				if (_keyIndex.TryGetValue(key, out actor))
					callback(actor);
				else
					notFoundCallback();
			});
		}

		public void Select(Uri actorAddress, Action<ActorRef> callback, Action notFoundCallback)
		{
			_fiber.Add(() =>
			{
				try
				{
					foreach (RegistryNode node in _nodes)
					{
						ActorRef actor = node.Select(actorAddress);
						if (actor != null)
						{
							callback(actor);
							return;
						}
					}
				}
				catch {}

				notFoundCallback();
			});
		}

		public void Each(Action<Guid, ActorRef> callback)
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

		public void AddNode(RegistryNode registryNode)
		{
			_nodes.Add(registryNode);
		}

		void Add(Guid key, ActorRef actor)
		{
			_keyIndex.Add(key, actor);
			_actors.Add(actor, key);

			_events.Send(new ActorRegisteredImpl(this, actor, key));
		}

		void Remove(Guid key, ActorRef actor)
		{
			_keyIndex.Remove(key);
			_actors.Remove(actor);

			_events.Send(new ActorUnregisteredImpl(this, actor, key));
		}

	    public void Send<T>(Message<T> message)
	    {
            _channel.Send(message);
        }
	}
}