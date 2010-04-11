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
namespace Magnum.Channels
{
	using System.Collections.Generic;

	public class KeyedChannelProvider<T, TKey> :
		ChannelProvider<T>
	{
		private readonly Dictionary<TKey, Channel<T>> _dictionary = new Dictionary<TKey, Channel<T>>();
		private readonly KeyAccessor<T, TKey> _keyAccessor;

		public KeyedChannelProvider(ChannelProvider<T> instanceProvider, KeyAccessor<T, TKey> keyAccessor)
		{
			InstanceProvider = instanceProvider;
			_keyAccessor = keyAccessor;
		}

		public ChannelProvider<T> InstanceProvider { get; private set; }

		public Channel<T> GetChannel(T message)
		{
			TKey key = _keyAccessor(message);

			Channel<T> value;
			if (_dictionary.TryGetValue(key, out value))
				return value;

			value = InstanceProvider.GetChannel(message);

			_dictionary.Add(key, value);

			return value;
		}
	}
}