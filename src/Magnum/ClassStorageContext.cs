using System;
using System.Collections.Generic;

namespace Magnum.Specs.Integration
{
    public class ClassStorageContext<T, K> : IClassStorageContext where T : class
    {
        private readonly KeyMap<T, K> _keyMap;
        private readonly Dictionary<K, T> _storage = new Dictionary<K, T>();

        public ClassStorageContext(ObjectToKey<T, K> objectToKey)
        {
            _keyMap = new KeyMap<T, K>(objectToKey);
        }

        public void Save<T1>(T1 item)
        {
            if (typeof (T1) != typeof (T))
                throw new NotImplementedException();

            K key = _keyMap.GetKeyFromObject(item);

            if (_storage.ContainsKey(key))
                _storage[key] = item as T;
            else
                _storage.Add(key, item as T);
        }

        public T1 Get<T1>(object id) where T1 : class
        {
            if (typeof (T1) != typeof (T))
                throw new NotImplementedException();

            K key = _keyMap.GetKeyFromId(id);


            if (_storage.ContainsKey(key))
                return _storage[key] as T1;

            return default(T1);
        }

        public IList<T1> List<T1>() where T1 : class
        {
            List<T1> result = new List<T1>();

            foreach (T t in _storage.Values)
            {
                result.Add(t as T1);
            }

            return result;
        }
    }
}