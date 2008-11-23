namespace Magnum.ProtocolBuffers.Mapping
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Common.Reflection;
    using Specs;

    public class FieldDescriptor
    {
        private readonly FastProperty _fastProp;

        public FieldDescriptor(string name, int numberTag, PropertyInfo func, FieldRules rules)
        {
            Name = name;
            Rules = rules;
            NumberTag = numberTag;
            PropertyInfo = func;
            FieldType = PropertyInfo.PropertyType;
            _fastProp = new FastProperty(PropertyInfo);
        }
        public FieldDescriptor(string name, int numberTag, PropertyInfo func, FieldRules rules, object defaultValue) :
            this(name, numberTag, func, rules)
        {
            DefaultValue = defaultValue;
            HasDefaultValue = true;
        }

        public string Name { get; private set; }
        public int NumberTag { get; private set; }
        public FieldRules Rules { get; private set; }
        public Type FieldType { get; private set; }
        public object DefaultValue { get; private set; }
        public bool HasDefaultValue { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }

        public void SetFieldOnInstance(object instance, object value)
        {
            _fastProp.Set(instance, value);
        }
        public object GetFieldOnInstance(object instance)
        {
            return _fastProp.Get(instance);
        }
    }
}