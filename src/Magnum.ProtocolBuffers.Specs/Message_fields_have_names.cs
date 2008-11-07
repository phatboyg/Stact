namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using NUnit.Framework;

    [TestFixture]
    public class Message_fields_have_names
    {
        [Test]
        public void Name_should_be_set_automatically()
        {
            Expression<Func<TestMessage, string>> expression = m => m.Name;
            var propInfo = ReflectionHelper.GetProperty(expression);
            var mapping = new FieldMap(propInfo, 1);
            Assert.AreEqual("Name", mapping.FieldName);
        }
    }
}