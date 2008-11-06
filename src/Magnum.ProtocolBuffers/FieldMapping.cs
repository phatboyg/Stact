namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Linq.Expressions;
    using Specs;

    public class FieldMapping<TMessage, TPropertyValue>
    {
        private FieldRules _rules;
        private Type _fieldType;
        private readonly string _fieldName;
        private TPropertyValue _defaultValue;
        private bool _hasDefaultValue;

        public FieldMapping(Expression<Func<TMessage, TPropertyValue>> expression, int numberTag)
        {
            NumberTag = numberTag;
            _rules = FieldRules.Optional;


            PopulateFieldSettings();
            
            SetConventionalFieldRules();

            var body = (MemberExpression)expression.Body;
            _fieldName = body.Member.Name;
        }

        private void PopulateFieldSettings()
        {
            _fieldType = typeof(TPropertyValue);

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

        public TPropertyValue DefaultValue
        {
            get { return _defaultValue; }
        }

        public bool HasDefaultValue
        {
            get { return _hasDefaultValue; }
        }

        private void SetConventionalFieldRules()
        {
            if(typeof(TPropertyValue).IsCollection())
            {
                MakeRepeated();
            }

            if(typeof(TPropertyValue).IsValueType && !typeof(TPropertyValue).IsGenericType)
            {
                MakeRequired();
            }
        }

        public void SetDefaultValue(TPropertyValue value)
        {
            _defaultValue = value;
            _hasDefaultValue = true;
        }
    }

    
}