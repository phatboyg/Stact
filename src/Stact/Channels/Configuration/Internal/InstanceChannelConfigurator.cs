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
namespace Stact.Configuration.Internal
{
	using System;


	public interface InstanceChannelConfigurator<TChannel> :
		ChannelConfigurator<TChannel>
	{
		InstanceChannelConfigurator<TInstance, TChannel> Of<TInstance>()
			where TInstance : class;
	}


	/// <summary>
	/// A fluent syntax for configuration options of a channel consumer
	/// </summary>
	/// <typeparam name="TInstance">The consumer type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public interface InstanceChannelConfigurator<TInstance, TChannel>
		where TInstance : class
	{
		void SetProviderFactory(Func<ConnectionBuilder<TChannel>,ChannelProvider<TChannel>> providerFactory);
	}


	public interface InstanceChannelProviderConfigurator<TInstance, TChannel>
		where TInstance : class
	{
		InstanceChannelProviderConfigurator<TInstance, TChannel> OnChannel(
			ChannelAccessor<TInstance, TChannel> accessor);
	}
}