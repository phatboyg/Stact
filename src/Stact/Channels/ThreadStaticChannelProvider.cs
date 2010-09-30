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
namespace Stact.Channels
{
	using System;

	/// <summary>
	/// Provides an instance of a channel per thread for situations where the
	/// processing of a channel should be done per-thread to avoid bottlenecks
	/// 
	/// This should not be used in cases where the channel maintains state
	/// between calls since threads may change and ordering cannot be guaranteed
	/// </summary>
	/// <typeparam name="T">The message type of the channel</typeparam>
	public class ThreadStaticChannelProvider<T> :
		ChannelProvider<T>
	{
		[ThreadStatic]
		private static Channel<T> _instance;

		public ThreadStaticChannelProvider(ChannelProvider<T> instanceProvider)
		{
			InstanceProvider = instanceProvider;
		}

		public ChannelProvider<T> InstanceProvider { get; private set; }

		public Channel<T> GetChannel(T message)
		{
			if (_instance == null)
			{
				_instance = InstanceProvider.GetChannel(message);
			}

			return _instance;
		}
	}
}