using System;
using System.ComponentModel;

namespace Magnum.Specs.Integration
{
    public class KeyMap<T, K> where T : class
    {
        private readonly ObjectToKey<T, K> _objectToKey;
        private readonly TypeConverter _tc = new TypeConverter();

        public KeyMap(ObjectToKey<T, K> objectToKey)
        {
            _objectToKey = objectToKey;
        }

        public K GetKeyFromObject(object obj)
        {
            return _objectToKey(obj as T);
        }

        public K GetKeyFromId(object id)
        {
            if (typeof (K).IsAssignableFrom(id.GetType()))
                return (K) id;

            throw new ApplicationException("Invalid key type specified");
        }
    }
}