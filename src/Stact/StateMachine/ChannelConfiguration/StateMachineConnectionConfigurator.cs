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
namespace Stact.StateMachine.ChannelConfiguration
{
	using System;
	using Configuration;
	using Magnum.StateMachine;


	public interface StateMachineConnectionConfigurator<T>
		where T : StateMachine<T>
	{
		StateMachineConnectionConfigurator<T, TKey, TBinding> BindUsing<TBinding, TKey>()
			where TBinding : StateMachineBinding<T, TKey>;

		StateMachineInstanceConnectionConfigurator<T> UsingInstance(T instance);
	}


	public interface StateMachineConnectionConfigurator<T, TKey, TBinding> :
		FiberProviderConfigurator<StateMachineConnectionConfigurator<T, TKey, TBinding>, TKey>
		where T : StateMachine<T>
	{
		void SetNewInstanceFactory(Func<TKey, T> instanceFactory);
		Func<TKey, T> GetConfiguredInstanceFactory();

		FiberProvider<TKey> GetConfiguredProvider();

		void SetProviderFactory(ChannelProviderFactory<T, TKey> factory);
	}
}