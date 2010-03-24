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

	public class DictionaryChannelInstanceCache<TChannel, TKey> :
		ChannelInstanceCache<TChannel>
	{
		private readonly Dictionary<TKey, Channel<TChannel>> _dictionary = new Dictionary<TKey, Channel<TChannel>>();
		private readonly ChannelInstanceProvider<TChannel> _instanceProvider;
		private readonly KeyProvider<TChannel, TKey> _keyProvider;

		public DictionaryChannelInstanceCache(ChannelInstanceProvider<TChannel> instanceProvider, KeyProvider<TChannel, TKey> keyProvider)
		{
			_instanceProvider = instanceProvider;
			_keyProvider = keyProvider;
		}

		public Channel<TChannel> Get(TChannel message)
		{
			TKey key = _keyProvider(message);

			Channel<TChannel> value;
			if (_dictionary.TryGetValue(key, out value))
				return value;

			value = _instanceProvider(message);

			_dictionary.Add(key, value);

			return value;
		}
	}
}