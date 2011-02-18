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
namespace Stact.Remote
{
	using System;
	using Configuration;
	using Events;
	using Internal;
	using Magnum.Serialization;
	using ReliableMulticast;


	public class ReliableMulticastRemoteActorRegistry :
		RemoteActorRegistry
	{
		readonly ActorRegistry _registry;
		readonly Serializer _serializer;
		readonly Uri _listenUri;
		ChunkChannel _reader;
		ReliableMulticastListener _listener;
		Scheduler _scheduler;

		public ReliableMulticastRemoteActorRegistry(ActorRegistry registry, Serializer serializer, Uri listenUri)
		{
			_registry = registry;
			_serializer = serializer;
			_listenUri = listenUri;
			_scheduler = new TimerScheduler(new PoolFiber());

			_reader = new ChunkChannel(this, serializer);
			_listener = new ReliableMulticastListener(_listenUri, _reader);
		}

		public void Start()
		{
			_listener.Start();
		}

		public void Send<T>(T message)
		{
			_registry.Send(message);
		}

		public void Register(Guid key, ActorInstance actor)
		{
			_registry.Register(key, actor);
		}

		public void Register(ActorInstance actor, Action<Guid, ActorInstance> callback)
		{
			_registry.Register(actor, callback);
		}

		public void Unregister(ActorInstance actor)
		{
			_registry.Unregister(actor);
		}

		public void Unregister(Guid key)
		{
			_registry.Unregister(key);
		}

		public void Shutdown()
		{
			_registry.Shutdown();

			_listener.Dispose();
		}

		public void Get(Guid key, Action<ActorInstance> callback, Action notFoundCallback)
		{
			_registry.Get(key, callback, notFoundCallback);
		}

		public void Each(Action<Guid, ActorInstance> callback)
		{
			_registry.Each(callback);
		}

		public ChannelConnection Subscribe(Action<ConnectionConfigurator> subscriberActions)
		{
			return _registry.Subscribe(subscriberActions);
		}

		public ChannelConnection Subscribe(Channel<ActorRegistered> listener)
		{
			return _registry.Subscribe(listener);
		}

		public ChannelConnection Subscribe(Channel<ActorUnregistered> listener)
		{
			return _registry.Subscribe(listener);
		}

		public ChannelConnection Subscribe(Channel<ActorRegistered> registeredListener,
		                                   Channel<ActorUnregistered> unregisteredListener)
		{
			return _registry.Subscribe(registeredListener, unregisteredListener);
		}

		public ActorInstance Select(Uri remoteAddress)
		{
			var actor = AnonymousActor.New(inbox =>
				{
					Console.WriteLine("Starting writer");

					var writer = new ReliableMulticastWriter(remoteAddress);
					writer.Start();

					var intercepter = new DelegateChunkWriter(chunk =>
						{
							Console.WriteLine(chunk.ToMemoryViewString());

							writer.Write(chunk, x =>
								{
								});
						});

					Console.WriteLine("Starting buffer");

					var buffer = new BufferedChunkWriter(new PoolFiber(), _scheduler, intercepter, 64*1024);
					buffer.Start();

					var chunkWriterChannel = new ChunkWriterChannel(buffer, _serializer);
					var chunkHeaderChannel = new ChunkHeaderChannel(chunkWriterChannel);

					var channel = new HeaderChannelAdapter(chunkHeaderChannel);

					// TODO need to decorate this channel to remote the host/port from uri
					// and convert to a urn:actor:xxxxxxxx-xxxx....

					ChannelConnection connection = null;
					connection = inbox.Connect(x =>
						{
							x.AddChannel(channel);

							x.AddConsumerOf<Request<Exit>>()
								.UsingConsumer(exit =>
									{
										connection.Dispose();
										buffer.Dispose();
										writer.Dispose();
									});
						});
				});

			return actor;
		}
	}
}