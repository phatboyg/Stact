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
namespace Stact.Channels
{
	using System;
	using Magnum.Extensions;


	/// <summary>
	/// Gets an instance of a class from the InstanceProvider and returns the channel
	/// from that class
	/// </summary>
	/// <typeparam name="TInstance">The instance type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public class InstanceChannelProvider<TInstance, TChannel> :
		ChannelProvider<TChannel>
		where TInstance : class
	{
		readonly ChannelAccessor<TInstance, TChannel> _accessor;
		readonly InstanceProvider<TInstance, TChannel> _instanceProvider;

		public InstanceChannelProvider(InstanceProvider<TInstance, TChannel> instanceProvider,
		                               ChannelAccessor<TInstance, TChannel> accessor)
		{
			Magnum.Guard.AgainstNull(instanceProvider);
			Magnum.Guard.AgainstNull(accessor);

			_instanceProvider = instanceProvider;
			_accessor = accessor;
		}

		public Channel<TChannel> GetChannel(TChannel message)
		{
			TInstance instance = _instanceProvider.GetInstance(message);

			Channel<TChannel> channel = _accessor(instance);
			if (channel == null)
			{
				throw new InvalidOperationException("The channel on the consumer {0} is null: {1}"
				                                    	.FormatWith(typeof(TInstance).Name, typeof(TChannel).Name));
			}

			return channel;
		}
	}
}