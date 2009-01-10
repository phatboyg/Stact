namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Common.Reflection;
    using Mapping;
    using Streams;

    public class FieldSerializer
    {
        private readonly FastProperty _funcs;
        private readonly ISerializationStrategy _strategy;
        public FieldSerializer(FieldRules rules, int fieldTag, FastProperty func, Type netType, ISerializationStrategy strategy)
        {
            Rules = rules;
            FieldTag = fieldTag;
            _funcs = func;
            NetType = netType;
            _strategy = strategy;
        }

        public FieldRules Rules { get; set;}
        public int FieldTag { get; set; }
        public Type NetType { get; set; }

        public void Serialize(CodedOutputStream stream, object instance)
        {
            object valueToSerialize = _funcs.Get(instance);
            _strategy.Serialize(stream, FieldTag, valueToSerialize);
        }
        public void Deserialize(CodedInputStream stream, ref object instance)
        {
            var deserializedValue = _strategy.Deserialize(stream);
            SetField(instance, deserializedValue);
        }

        public void SetField(object instance, object value)
        {
            _funcs.Set(instance, value);
        }

        public object GetFieldValue(object instance)
        {
            return _funcs.Get(instance);
        }
    }
}