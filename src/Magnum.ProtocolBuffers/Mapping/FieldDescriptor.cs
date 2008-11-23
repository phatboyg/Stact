namespace Magnum.ProtocolBuffers.Mapping
{
    using System;
    using System.Reflection;
    using Common.Reflection;
    using Serialization;
    using Serialization.Strategies;
    using Specs;

    public class FieldDescriptor
    {

        public FieldDescriptor(string name, int numberTag, PropertyInfo func, FieldRules rules)
        {
            Name = name;
            Rules = rules;
            NumberTag = numberTag;
            PropertyInfo = func;
            NetType = PropertyInfo.PropertyType;
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
        public Type NetType { get; private set; }
        public object DefaultValue { get; private set; }
        public bool HasDefaultValue { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public bool IsRepeated
        {
            get { return Rules.Equals(FieldRules.Repeated); }
        }

        public FieldSerializer GenerateFieldSerializer()
        {
            ISerializationStrategy strategy = null;
            
            Type netType = NetType;
            
            if (IsRepeated)
            {
                netType = NetType.GetTypeEnumerated();
                strategy = new ListStrategy(new MessageStrategy(null));
            }

            return new FieldSerializer()
                       {
                           FieldTag = NumberTag,
                           Func = new FastProperty(PropertyInfo),
                           NetType = netType,
                           Rules = Rules,
                           Strategy = strategy,
                       };
        }
        
    }
}