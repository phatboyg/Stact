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


	public class RemoteActorRegistryConfiguratorImpl :
		FiberFactoryConfiguratorImpl<RemoteActorRegistryConfigurator>,
		RemoteActorRegistryConfigurator,
		RegistryBuilderConfigurator
	{
		Uri _listenUri;
		Func<Serializer> _serializerFactory;

		public RemoteActorRegistryConfiguratorImpl()
		{
			UseSerializerFactory(() => new FastTextSerializer());
			HandleOnPoolFiber();
		}

		public RegistryBuilder Configure(RegistryBuilder builder)
		{
			var remoteBuilder = new RemoteActorRegistryBuilder(builder, _listenUri, GetConfiguredFiberFactory(), _serializerFactory);

			return remoteBuilder;
		}

		public void ValidateConfiguration()
		{
			ValidateFiberFactoryConfiguration();

			if (_listenUri == null)
				throw new StactException("A Uri must be specified for the remote actor registry");

			if (_serializerFactory == null)
				throw new StactException("A serializer must be specified for the remote actor registry");
		}

		public RemoteActorRegistryConfigurator ListenTo(Uri uri)
		{
			_listenUri = uri;

			return this;
		}

		public RemoteActorRegistryConfigurator UseSerializerFactory(Func<Serializer> serializerFactory)
		{
			_serializerFactory = serializerFactory;

			return this;
		}
	}
}