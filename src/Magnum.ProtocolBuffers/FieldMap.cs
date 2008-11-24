namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Common;
    using Specs;

    public class FieldMap<TMessage>
    {
        private static readonly Range<int> _googlesFieldNumbers = new Range<int>(19000, 19999, true, true);
        private readonly string _name;
        private object _defaultValue;
        private Type _fieldType;
        private bool _hasDefaultValue;
        private FieldRules _rules;
        private Expression<Func<TMessage, object>> _func;
        private PropertyInfo _propertyInfo;

        public FieldMap(int numberTag, Expression<Func<TMessage, object>> func)
        {
            if (_googlesFieldNumbers.Contains(numberTag))
                throw new ProtoMappingException(string.Format("A field mapping cannot have a numberTag between 19000 and 19999. Its owned by google!"));

            _func = func;
            _propertyInfo = ReflectionHelper.GetProperty(func);

            NumberTag = numberTag;
            _rules = FieldRules.Optional;


            PopulateFieldSettings(_propertyInfo);

            SetConventionalFieldRules();

            _name = _propertyInfo.Name;
        }


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

        public PropertyInfo PropertyInfo
        {
            get { return _propertyInfo; }
        }

        private void PopulateFieldSettings(PropertyInfo info)
        {
            _fieldType = info.PropertyType;

            if (_fieldType.IsEnum)
                throw new NotSupportedException("Fluent Proto Buffers does not yet support enumerations");
        }

        public FieldMap<TMessage> MakeRequired()
        {
            _rules = FieldRules.Required;

            return this;
        }

        public void MakeRepeated()
        {
            _rules = FieldRules.Repeated;
        }

        private void SetConventionalFieldRules()
        {
            if (_fieldType.IsRepeatedType())
            {
                MakeRepeated();
            }

            if (_fieldType.IsRequiredType())
            {
                MakeRequired();
            }
        }

        public void SetDefaultValue(object value)
        {
            _defaultValue = value;
            _hasDefaultValue = true;
        }
    }
}