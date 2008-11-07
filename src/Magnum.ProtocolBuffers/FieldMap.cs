namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Specs;

    public class FieldMap
    {
        private FieldRules _rules;
        private Type _fieldType;
        private readonly string _fieldName;
        private object _defaultValue;
        private bool _hasDefaultValue;

        public FieldMap(PropertyInfo propertyInfo, int numberTag)
        {
            NumberTag = numberTag;
            _rules = FieldRules.Optional;


            PopulateFieldSettings(propertyInfo);
            
            SetConventionalFieldRules();

            _fieldName = propertyInfo.Name;
        }

        private void PopulateFieldSettings(PropertyInfo info)
        {
            _fieldType = info.PropertyType;

            if(_fieldType.IsEnum)
                throw new NotSupportedException("Fluent Proto Buffers does not yet support enumerations");
        }


        public string FieldName
        {
            get { return _fieldName; }
        }

        public int NumberTag { get; private set; }

        public FieldRules Rules
        {
            get { return _rules; }
        }

        public void MakeRequired()
        {
            _rules = FieldRules.Required;
        }

        public void MakeRepeated()
        {
            _rules = FieldRules.Repeated;
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

        private void SetConventionalFieldRules()
        {
            if(_fieldType.IsRepeatedType())
            {
                MakeRepeated();
            }

            if(_fieldType.IsRequiredType())
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