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
namespace Stact.Configuration
{
	using System;
	using RegistryConfigurators;
	using Remote;


	public class RemoteActorRegistryBuilder :
		RegistryBuilder
	{
		readonly RegistryBuilder _builder;
		readonly FiberFactory _fiberFactory;
		readonly Uri _listenUri;
		readonly Func<Serializer> _serializerFactory;

		public RemoteActorRegistryBuilder(RegistryBuilder builder, Uri listenUri, FiberFactory fiberFactory,
		                                  Func<Serializer> serializerFactory)
		{
			_builder = builder;
			_serializerFactory = serializerFactory;
			_listenUri = listenUri;
			_fiberFactory = fiberFactory;
		}

		public Fiber Fiber
		{
			get { return _builder.Fiber; }
		}

		public Scheduler Scheduler
		{
			get { return _builder.Scheduler; }
		}

		public ActorRegistry Build()
		{
			ActorRegistry registry = _builder.Build();

			Serializer serializer = _serializerFactory();

			Scheduler scheduler = _builder.Scheduler;

			var registryNode = new RemoteRegistryNode(registry, _listenUri, _fiberFactory, scheduler, serializer);

			registry.AddNode(registryNode);

			return registry;
		}
	}
}