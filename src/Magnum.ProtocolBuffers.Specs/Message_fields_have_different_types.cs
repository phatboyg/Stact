namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
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
            var prop = ReflectionHelper.GetProperty(basicField);
            var mapping = new FieldMap(prop,1);
            Assert.AreEqual(FieldRules.Optional, mapping.Rules);
        }
        [Test]
        public void Field_should_self_set_repeated()
        {
            var prop = ReflectionHelper.GetProperty(repeatedField);
            var mapping = new FieldMap(prop, 1);
            Assert.AreEqual(FieldRules.Repeated, mapping.Rules);
        }

        [Test]
        public void Field_should_self_set_required()
        {
            var prop = ReflectionHelper.GetProperty(valueTypeField);
            var mapping = new FieldMap(prop, 1);
            Assert.AreEqual(FieldRules.Required, mapping.Rules);

            var prop2 = ReflectionHelper.GetProperty<TestMessage, DateTime>(m=>m.BirthDay);
            var mapping2 = new FieldMap(prop2, 1);
            Assert.AreEqual(FieldRules.Required, mapping2.Rules);
        }

        [Test]
        public void Nullable_fields_should_be_optional_by_default()
        {
            var prop = ReflectionHelper.GetProperty(nullableField);
            var mapping = new FieldMap(prop,1);
            Assert.AreEqual(FieldRules.Optional, mapping.Rules);

            var prop2 = ReflectionHelper.GetProperty<TestMessage, DateTime?>(m => m.DeadDay);
            var mapping2 = new FieldMap(prop2, 1);
            Assert.AreEqual(FieldRules.Optional, mapping2.Rules);
        }
    }
}