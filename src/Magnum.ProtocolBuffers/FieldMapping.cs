namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Linq.Expressions;

    public class FieldMapping<TMessage>
    {
        private FieldRules _rules;

        public FieldMapping(Expression<Func<TMessage, string>> function, int numberTag)
        {
            NumberTag = numberTag;
            _rules = Specs.FieldRules.Optional;
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
    }
}