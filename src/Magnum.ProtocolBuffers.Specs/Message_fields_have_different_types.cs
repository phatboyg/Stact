namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class Message_fields_have_different_types :
        Specification
    {
        private readonly Expression<Func<TestMessage, string>> basicField = m => m.Name;
        private readonly Expression<Func<TestMessage, int>> valueTypeField = m => m.Age;
        private readonly Expression<Func<TestMessage, IList<int>>> repeatedField = m => m.Numbers;
        private readonly Expression<Func<TestMessage, int?>> nullableField = m => m.NumberOfPets;

        [Test]
        public void Fields_should_be_optional_by_default()
        {
            var mapping = new FieldMapping<TestMessage, string>(basicField,1);
            Assert.AreEqual(FieldRules.Optional, mapping.Rules);
        }
        [Test]
        public void Field_should_self_set_repeated()
        {
            var mapping = new FieldMapping<TestMessage, IList<int>>(repeatedField, 1);
            Assert.AreEqual(FieldRules.Repeated, mapping.Rules);
        }

        [Test]
        public void Field_should_self_set_required()
        {
            var mapping = new FieldMapping<TestMessage, int>(valueTypeField, 1);
            Assert.AreEqual(FieldRules.Required, mapping.Rules);

            var mapping2 = new FieldMapping<TestMessage, DateTime>(m => m.BirthDay, 1);
            Assert.AreEqual(FieldRules.Required, mapping2.Rules);
        }

        [Test]
        public void Nullable_fields_should_be_optional_by_default()
        {
            var mapping = new FieldMapping<TestMessage, int?>(nullableField,1);
            Assert.AreEqual(FieldRules.Optional, mapping.Rules);

            var mapping2 = new FieldMapping<TestMessage, DateTime?>(m => m.DeadDay, 1);
            Assert.AreEqual(FieldRules.Optional, mapping2.Rules);
        }
    }
}