namespace Magnum.ProtocolBuffers.Serialization
{
    using System;
    using Common.Reflection;
    using Specs;

    public class FieldSerializer
    {
        private readonly FastProperty _funcs;
        public FieldSerializer(FieldRules rules, int fieldTag, FastProperty func, Type netType, ISerializationStrategy strategy)
        {
            Rules = rules;
            FieldTag = fieldTag;
            _funcs = func;
            NetType = netType;
            Strategy = strategy;
        }

        public FieldRules Rules { get; set;}
        public int FieldTag { get; set; }
        public Type NetType { get; set; }
        public ISerializationStrategy Strategy { get; set; }

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