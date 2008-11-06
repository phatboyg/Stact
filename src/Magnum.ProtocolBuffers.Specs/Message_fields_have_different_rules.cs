namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;

    public class Message_fields_have_different_rules :
        Specification
    {
        private Expression<Func<TestMessage, string>> function = m => m.Name;

        [Test]
        public void Mappings_are_optional_by_default()
        {
            var fieldMapping = new FieldMapping<TestMessage>(function, 1);

            Assert.AreEqual(FieldRules.Optional, fieldMapping.Rules);
        }

        [Test]
        public void Mappings_can_be_made_required()
        {
            var fieldMapping = new FieldMapping<TestMessage>(function, 1);
            fieldMapping.MakeRequired();

            Assert.AreEqual(FieldRules.Required, fieldMapping.Rules);
        }

        [Test]
        public void Mappings_can_be_made_repeated()
        {
            var fieldMapping = new FieldMapping<TestMessage>(function, 1);
            fieldMapping.MakeRepeated();

            Assert.AreEqual(FieldRules.Repeated, fieldMapping.Rules);
        }
    }
}