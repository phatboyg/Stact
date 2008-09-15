using System;
using System.Collections.Generic;

namespace Magnum.Specs.Integration
{
    public class StorageContext
    {
        private readonly Dictionary<Type, IClassStorageContext> _classStorage = new Dictionary<Type, IClassStorageContext>();

        public void Save<T>(T item) where T : class
        {
            if (_classStorage.ContainsKey(typeof (T)))
                _classStorage[typeof (T)].Save(item);
            else
                throw new ApplicationException("No class type supported");
        }

        public T Get<T>(object id) where T : class
        {
            if (_classStorage.ContainsKey(typeof (T)))
                return _classStorage[typeof (T)].Get<T>(id);

            throw new ApplicationException("No class type supported");
        }

        public void RegisterClass<T, K>(ObjectToKey<T, K> objectToKey) where T : class
        {
            IClassStorageContext context = new ClassStorageContext<T, K>(objectToKey);
            _classStorage.Add(typeof (T), context);
        }

        public IList<T> List<T>() where T : class
        {
            if (_classStorage.ContainsKey(typeof (T)))
                return _classStorage[typeof (T)].List<T>();

            return new List<T>();
        }
    }
}