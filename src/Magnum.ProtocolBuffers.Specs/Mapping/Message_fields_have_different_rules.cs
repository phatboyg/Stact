namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;
    using TestMessages;

    public class Message_fields_have_different_rules :
        Specification
    {
        private Expression<Func<TestMessage, object>> function = m => m.Name;
        private FieldMap<TestMessage> _fieldMap;

        protected override void Before_each()
        {
            var prop = ReflectionHelper.GetProperty(function);
            _fieldMap = new FieldMap<TestMessage>(1, function);
        }
        [Test]
        public void Mappings_are_optional_by_default()
        {
            Assert.AreEqual(FieldRules.Optional, _fieldMap.Rules);
        }

        [Test]
        public void Mappings_can_be_made_required()
        {
            _fieldMap.MakeRequired();

            Assert.AreEqual(FieldRules.Required, _fieldMap.Rules);
        }

        [Test]
        public void Mappings_can_be_made_repeated()
        {
            _fieldMap.MakeRepeated();

            Assert.AreEqual(FieldRules.Repeated, _fieldMap.Rules);
        }
    }
}