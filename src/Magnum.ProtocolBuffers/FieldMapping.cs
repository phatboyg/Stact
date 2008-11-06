namespace Magnum.ProtocolBuffers
{
    using System;
    using System.Linq.Expressions;
    using Specs;

    public class FieldMapping<TMessage, TPropertyValue>
    {
        private FieldRules _rules;
        private Type _propertyType;
        private readonly string _fieldName;

        public FieldMapping(Expression<Func<TMessage, TPropertyValue>> expression, int numberTag)
        {
            NumberTag = numberTag;
            _rules = FieldRules.Optional;
            _propertyType = typeof(TPropertyValue);
            
            SetConventionalFieldRules();

            var body = (MemberExpression)expression.Body;
            _fieldName = body.Member.Name;
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
    }

    
}