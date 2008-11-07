namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Linq.Expressions;
    using NUnit.Framework;

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
    }
}