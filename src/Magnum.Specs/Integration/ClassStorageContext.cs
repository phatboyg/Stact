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
namespace Magnum.Specs.Integration
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Common.Repository;

    public class ClassStorageContext<T, K> :
        RepositoryBase<T, K>
        where T : class, IAggregateRoot<K>
    {
        private readonly Dictionary<K, T> _storage = new Dictionary<K, T>();

        protected override IQueryable<T> RepositoryQuery
        {
            get { return _storage.Values.AsQueryable(); }
        }

        public override void Save(T item)
        {
            if (_storage.ContainsKey(item.Id))
                _storage[item.Id] = item;
            else
                _storage.Add(item.Id, item);
        }

        public override void Update(T item)
        {
            _storage[item.Id] = item;
            
        }

        public override void Delete(T item)
        {
            _storage.Remove(item.Id);
        }

        public override void Dispose()
        {
            _storage.Clear();
        }

        public override T Get(K id)
        {
            T ret;
            if (!_storage.TryGetValue(id, out ret))
            {
                ret = default(T);
            }
            return ret;
        }

        public override IList<T> List()
        {
            List<T> result = new List<T>();

            foreach (T t in _storage.Values)
            {
                result.Add(t);
            }

            return result;
        }
    }
}