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
namespace Magnum.Channels.Internal
{
	using System;

	public interface SubscriptionConfigurator
	{
		void Add<TChannel>(Channel<TChannel> channel);


		ChannelSubscriptionConfigurator<TChannel> Consume<TChannel>();
	}


	public interface ChannelSubscriptionConfigurator<TChannel>
	{
		/// <summary>
		/// Defines a consumer that is to consume the specified message type
		/// </summary>
		/// <typeparam name="TConsumer"></typeparam>
		/// <param name="channelAccessor"></param>
		/// <returns></returns>
		ConsumerSubscriptionConfigurator<TConsumer, TChannel> Using<TConsumer>(ChannelAccessor<TConsumer, TChannel> channelAccessor);


		ChannelSubscriptionConfigurator<TChannel> Using(SelectiveConsumer<TChannel> consumer);


		ChannelSubscriptionConfigurator<TChannel> Using(Consumer<TChannel> consumer);
	}

	public interface ConsumerSubscriptionConfigurator<TConsumer, TChannel>
	{
		ConsumerSubscriptionConfigurator<TConsumer, TChannel> ObtainedBy(Func<TChannel, TConsumer> consumerFactory);
		
	}
}