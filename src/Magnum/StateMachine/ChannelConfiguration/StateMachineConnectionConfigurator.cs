// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.StateMachine.ChannelConfiguration
{
	using System;
	using Magnum.Channels;
	using Magnum.Fibers;


	public interface StateMachineConnectionConfigurator<T>
		where T : StateMachine<T>
	{
		StateMachineConnectionConfigurator<T, TKey, TBinding> BindUsing<TBinding, TKey>()
			where TBinding : StateMachineBinding<T, TKey>;

		StateMachineInstanceConnectionConfigurator<T> UsingInstance(T instance);
	}


	public interface StateMachineConnectionConfigurator<T, TKey, TBinding>
		where T : StateMachine<T>
	{
		StateMachineConnectionConfigurator<T, TKey, TBinding> ExecuteOnProducerThread();
		StateMachineConnectionConfigurator<T, TKey, TBinding> ExecuteOnSharedFiber();
		StateMachineConnectionConfigurator<T, TKey, TBinding> ExecuteOnThreadPoolFiber();
		StateMachineConnectionConfigurator<T, TKey, TBinding> ExecuteOnSharedThread();
		StateMachineConnectionConfigurator<T, TKey, TBinding> UseFiberProvider(FiberProvider<TKey> fiberProvider);

		void SetNewInstanceFactory(Func<TKey, T> instanceFactory);
		Func<TKey, T> GetConfiguredInstanceFactory();

		FiberProvider<TKey> GetConfiguredProvider();

		void SetProviderFactory(ChannelProviderFactory<T,TKey> factory);
	}
}