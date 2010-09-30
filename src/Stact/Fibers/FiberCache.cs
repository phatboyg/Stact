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
namespace Stact.Fibers
{
	using System;
	using Magnum.Collections;
	using Magnum.Extensions;


	/// <summary>
	/// Keeps track of a keyed fiber collection
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public class FiberCache<TKey> :
		FiberProvider<TKey>
	{
		readonly Cache<TKey, Fiber> _cache;
		readonly TimeSpan _timeout = 60.Seconds();

		public FiberCache(FiberFactory missingFiberFactory)
		{
			_cache = new Cache<TKey, Fiber>(k => missingFiberFactory());
		}

		public Fiber GetFiber(TKey key)
		{
			return _cache[key];
		}

		public void Dispose()
		{
			_cache.Each(x => x.Shutdown(_timeout));
		}
	}
}