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
namespace Magnum.Channels.Configuration.Internal
{
	using Magnum.Fibers;


	/// <summary>
	/// An instance configuration that is distributed by a specified key
	/// </summary>
	/// <typeparam name="TInstance"></typeparam>
	/// <typeparam name="TChannel"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> :
		InstanceChannelConfigurator<TInstance, TChannel>
		where TInstance : class
	{
		DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> ExecuteOnProducerThread();
		DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> ExecuteOnSharedFiber();
		DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> ExecuteOnThreadPoolFiber();
		DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> ExecuteOnSharedThread();
		DistributedInstanceChannelConfigurator<TInstance, TChannel, TKey> UseFiberProvider(FiberProvider<TKey> fiberProvider);

		FiberProvider<TKey> GetConfiguredProvider();

		KeyAccessor<TChannel, TKey> GetDistributionKeyAccessor();
	}
}