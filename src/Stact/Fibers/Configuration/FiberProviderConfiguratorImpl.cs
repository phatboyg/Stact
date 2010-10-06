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
	using Fibers;


	public class FiberProviderConfiguratorImpl<T, TKey> :
		FiberFactoryConfiguratorImpl<T>,
		FiberProviderConfigurator<T, TKey>
		where T : class
	{
		Func<FiberFactory, TimeSpan, FiberProvider<TKey>> _configuredProvider;

		protected FiberProviderConfiguratorImpl()
		{
			CreateFiberPerInstance();
		}

		public T ShareFiberAcrossInstances()
		{
			_configuredProvider = (factory, timeout) => new SingleFiberProvider<TKey>(factory, timeout);

			return this as T;
		}

		public T CreateFiberPerInstance()
		{
			_configuredProvider = (factory, timeout) => new KeyedChannelProvider<TKey>(factory, timeout);

			return this as T;
		}

		public T UseFiberProvider(FiberProvider<TKey> fiberProvider)
		{
			_configuredProvider = (factory, timeout) => fiberProvider;

			return this as T;
		}

		protected FiberProvider<TKey> GetConfiguredFiberProvider()
		{
			if (_configuredProvider == null)
				throw new FiberConfigurationException("No provider specified for FiberProvider");

			FiberFactory fiberFactory = GetConfiguredFiberFactory();

			return _configuredProvider(fiberFactory, ShutdownTimeout);
		}
	}
}