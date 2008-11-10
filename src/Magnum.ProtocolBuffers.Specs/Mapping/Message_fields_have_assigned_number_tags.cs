namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Message_fields_have_assigned_number_tags :
        Specification
    {
        [Test]
        public void Mappings_need_number_tags()
        {
            Expression<Func<TestMessage, string>> function = m => m.Name;
            var prop = ReflectionHelper.GetProperty(function);

            var fieldMapping = new FieldMap(prop, 1);

            Assert.AreEqual(1, fieldMapping.NumberTag);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void You_cant_use_19000()
        {
            new FieldMap(null, 19000);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void You_cant_use_19999()
        {
            new FieldMap(null, 19999);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void Or_anything_between()
        {
            new FieldMap(null, 19500);
        }
    }
}