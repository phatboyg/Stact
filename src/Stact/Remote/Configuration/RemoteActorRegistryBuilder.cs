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
	using Magnum.Serialization;
	using RegistryConfigurators;
	using Remote;


	public class RemoteActorRegistryBuilder :
		RegistryBuilder
	{
		readonly RegistryBuilder _builder;
		readonly Uri _listenUri;
		readonly Func<Serializer> _serializerFactory;

		public RemoteActorRegistryBuilder(RegistryBuilder builder, Func<Serializer> serializerFactory, Uri listenUri)
		{
			_builder = builder;
			_serializerFactory = serializerFactory;
			_listenUri = listenUri;
		}

		public ActorRegistry Build()
		{
			ActorRegistry registry = _builder.Build();

			Serializer serializer = _serializerFactory();

			var remoteRegistry = new ReliableMulticastRemoteActorRegistry(registry, serializer, _listenUri);

			remoteRegistry.Start();

			return remoteRegistry;
		}
	}
}