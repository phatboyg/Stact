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
namespace Stact
{
	using System;

	/// <summary>
	/// Wraps an anonymous method in a ChannelProvider
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public class DelegateChannelProvider<T> :
		ChannelProvider<T>
	{
		private readonly Func<T, Channel<T>> _provider;

		public DelegateChannelProvider(Func<T, Channel<T>> provider)
		{
		    if (provider == null)
		        throw new ArgumentNullException("provider");

			_provider = provider;
		}

		public Channel<T> GetChannel(T message)
		{
			return _provider(message);
		}
	}
}