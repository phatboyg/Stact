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
    using System;
    using System.Collections.Generic;
    using Common;
    using Common.Repository;

    public class StorageContext 
    {
        private readonly Dictionary<Type, IDisposable> _classStorage = new Dictionary<Type, IDisposable>();

        public void Save<T>(T item) where T : class, IAggregateRoot<Guid>
        {
            WithRepository<T>(r => r.Save(item));
        }

        public void Delete<T>(T item) where T : class, IAggregateRoot<Guid>
        {
            WithRepository<T>(r => r.Delete(item));
        }

        public T Get<T>(Guid id) where T : class, IAggregateRoot<Guid>
        {
            return WithRepository<T>(r => r.Get(id));
        }

        public IList<T> List<T>() where T : class, IAggregateRoot<Guid>
        {
            return WithRepository<T>(r => r.List());
        }

        public void Dispose()
        {
            foreach (IDisposable disposable in _classStorage.Values)
            {
                using (disposable)
                {
                }
            }
            _classStorage.Clear();
        }

        private void WithRepository<T>(Action<IRepository<T,Guid>> action) where T : class, IAggregateRoot<Guid>
        {
            IDisposable classStorage;
            if (_classStorage.TryGetValue(typeof(T), out classStorage))
            {
                IRepository<T, Guid> repository = (IRepository<T, Guid>) classStorage;
                action(repository);
                return;
            }

            throw new ApplicationException("No class type supported");
        }

        private T WithRepository<T>(Func<IRepository<T,Guid>, T> action) where T : class, IAggregateRoot<Guid>
        {
            IDisposable classStorage;
            if (_classStorage.TryGetValue(typeof(T), out classStorage))
            {
                IRepository<T, Guid> repository = (IRepository<T, Guid>)classStorage;
                return action(repository);
            }

            throw new ApplicationException("No class type supported");
        }

       private IList<T> WithRepository<T>(Func<IRepository<T,Guid>, IList<T>> action) where T : class, IAggregateRoot<Guid>
        {
            IDisposable classStorage;
            if (_classStorage.TryGetValue(typeof(T), out classStorage))
            {
                IRepository<T, Guid> repository = (IRepository<T, Guid>)classStorage;
                return action(repository);
            }

            throw new ApplicationException("No class type supported");
        }

        public void RegisterClass<T, K>() where T : class, IAggregateRoot<K>
        {
            IRepository<T,K> context = new ClassStorageContext<T, K>();
            _classStorage.Add(typeof (T), context);
        }
    }
}