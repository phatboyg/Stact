namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using System.Collections.Generic;
    using Streams;

    public static class SerializerFor<T> where T : class, new()
    {
        private static readonly Dictionary<Type, object> _serializers = new Dictionary<Type, object>();
        
        public static void Add(object o)
        {
            _serializers.Add(typeof(T), o);
        }

        public static void Serialize(T instance, CodedOutputStream stream)
        {
            
        }

        public static T Deserialize(CodedInputStream stream)
        {
            return null;
        }
    }
}