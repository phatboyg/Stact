namespace Magnum.ProtocolBuffers.Mapping
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Specs;

    public class FieldDescriptor<TMessage>
    {
        private readonly string _name;
        private object _defaultValue;
        private Type _fieldType;
        private bool _hasDefaultValue;
        private FieldRules _rules;
        private Expression<Func<TMessage, object>> _func;
        private PropertyInfo _propertyInfo;

        public string Name
        {
            get { return _name; }
        }

        public int NumberTag { get; private set; }

        public FieldRules Rules
        {
            get { return _rules; }
        }

        public Type FieldType
        {
            get { return _fieldType; }
        }

        public object DefaultValue
        {
            get { return _defaultValue; }
        }

        public bool HasDefaultValue
        {
            get { return _hasDefaultValue; }
        }
        public Expression<Func<TMessage, object>> Lambda
        {
            get
            {
                return _func;
            }
        }

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }
    }
}