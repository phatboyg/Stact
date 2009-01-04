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
namespace Magnum.Common.Data
{
	using System.Collections.Generic;
	using System.Linq;
	using Threading;

	public class InMemoryRepository<T, K> :
		RepositoryBase<T, K>
		where T : class, IAggregateRoot<K>
	{
		private readonly ReaderWriterLockedObject<Dictionary<K, T>> _storage;

		public InMemoryRepository()
		{
			_storage = new ReaderWriterLockedObject<Dictionary<K, T>>(new Dictionary<K, T>());
		}

		protected override IQueryable<T> RepositoryQuery
		{
			get { return _storage.ReadLock(x => x.Values.AsQueryable()); }
		}

		public override void Dispose()
		{
			_storage.WriteLock(x => x.Clear());
		}

		public override T Get(K id)
		{
			return _storage.ReadLock(x => x.ContainsKey(id) ? x[id] : default(T));
		}

		public override IList<T> List()
		{
			return _storage.ReadLock(x => x.Values.ToList());
		}

		public override void Save(T item)
		{
			_storage.WriteLock(x =>
				{
					if (x.ContainsKey(item.Id))
						x[item.Id] = item;
					else
						x.Add(item.Id, item);
				});
		}

		public override void Update(T item)
		{
			_storage.WriteLock(x => { x[item.Id] = item; });
		}

		public override void Delete(T item)
		{
			_storage.WriteLock(x =>
				{
					if (x.ContainsKey(item.Id))
						x.Remove(item.Id);
				});
		}
	}
}