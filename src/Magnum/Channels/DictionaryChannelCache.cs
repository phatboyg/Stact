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

	public class DictionaryChannelCache<TChannel, TKey> :
		ChannelCache<TChannel>
	{
		private readonly Dictionary<TKey, Channel<TChannel>> _dictionary = new Dictionary<TKey, Channel<TChannel>>();
		private readonly ChannelProvider<TChannel> _instanceProvider;
		private readonly KeyAccessor<TChannel, TKey> _keyAccessor;

		public DictionaryChannelCache(ChannelProvider<TChannel> instanceProvider, KeyAccessor<TChannel, TKey> keyAccessor)
		{
			_instanceProvider = instanceProvider;
			_keyAccessor = keyAccessor;
		}

		public Channel<TChannel> Get(TChannel message)
		{
			TKey key = _keyAccessor(message);

			Channel<TChannel> value;
			if (_dictionary.TryGetValue(key, out value))
				return value;

			value = _instanceProvider(message);

			_dictionary.Add(key, value);

			return value;
		}
	}
}