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
namespace Magnum.Channels.Configuration
{
	using System;
	using System.Collections.Generic;
	using Reflection;

	public class ConsumerConfiguratorImpl<TConsumer, TChannel> :
		ConsumerConfigurator<TConsumer, TChannel>
	{
		private readonly ChannelAccessor<TConsumer, TChannel> _accessor;
		private readonly IList<object> _args = new List<object>();
		private Func<TChannel, TConsumer> _factory = DefaultConsumerFactory;

		public ConsumerConfiguratorImpl(ChannelAccessor<TConsumer, TChannel> accessor)
		{
			_accessor = accessor;
		}

		public ConsumerConfigurator<TConsumer, TChannel> ObtainedBy(Func<TConsumer> consumerFactory)
		{
			_factory = m => consumerFactory();

			return this;
		}

		public ConsumerConfigurator<TConsumer, TChannel> ObtainedBy(Func<TChannel, TConsumer> consumerFactory)
		{
			_factory = consumerFactory;

			return this;
		}

		public ChannelProvider<TChannel> GetChannelProvider()
		{
			return new InstanceChannelProvider<TConsumer, TChannel>(_factory, _accessor);
		}

		private static TConsumer DefaultConsumerFactory(TChannel message)
		{
			return FastActivator<TConsumer>.Create();
		}
	}
}