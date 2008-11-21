namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Message_fields_have_different_types :
        Specification
    {
        private readonly Expression<Func<TestMessage, object>> basicField = m => m.Name;
        private readonly Expression<Func<TestMessage, object>> valueTypeField = m => m.Age;
        private readonly Expression<Func<TestMessage, object>> repeatedField = m => m.Numbers;
        private readonly Expression<Func<TestMessage, object>> nullableField = m => m.NumberOfPets;

        

        [Test]
        public void Fields_should_be_optional_by_default()
        {
            var mapping = new FieldMap<TestMessage>( 1, basicField);
            Assert.AreEqual(FieldRules.Optional, mapping.Rules);
        }
        [Test]
        public void Field_should_self_set_repeated()
        {
            var mapping = new FieldMap<TestMessage>(1, repeatedField);
            Assert.AreEqual(FieldRules.Repeated, mapping.Rules);
        }

        [Test]
        public void Field_should_self_set_required()
        {
            var mapping = new FieldMap<TestMessage>(1, valueTypeField);
            Assert.AreEqual(FieldRules.Required, mapping.Rules);

            var mapping2 = new FieldMap<TestMessage>(1, m=>m.BirthDay);
            Assert.AreEqual(FieldRules.Required, mapping2.Rules);
        }

        [Test]
        public void Nullable_fields_should_be_optional_by_default()
        {
            var mapping = new FieldMap<TestMessage>(1, nullableField);
            Assert.AreEqual(FieldRules.Optional, mapping.Rules);

            var mapping2 = new FieldMap<TestMessage>(1, m=>m.DeadDay);
            Assert.AreEqual(FieldRules.Optional, mapping2.Rules);
        }
    }
}