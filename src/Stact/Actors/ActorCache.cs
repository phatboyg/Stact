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
namespace Stact.Actors
{
	using System;
	using Channels;
	using Collections;


	public class ActorCache<TActor, TKey> :
		ActorRepository<TActor, TKey>
		where TActor : Actor
	{
		private readonly Cache<TKey, TActor> _cache;
		private readonly KeyAccessor<TActor, TKey> _keyAccessor;

		public ActorCache(KeyAccessor<TActor, TKey> actorKeySelector, ActorProvider<TActor, TKey> actorProvider)
		{
			_keyAccessor = actorKeySelector;

			_cache = new Cache<TKey, TActor>(x => actorProvider(x));
		}

		public TActor Get(TKey key)
		{
			return _cache[key];
		}

		public TActor Get(TKey key, Func<TKey, TActor> onMissing)
		{
			throw new NotImplementedException();
		}
	}
}