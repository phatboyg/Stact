namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Message_fields_have_names
    {
        [Test]
        public void Name_should_be_set_automatically()
        {
            Expression<Func<TestMessage, object>> expression = m => m.Name;
            var mapping = new FieldMap<TestMessage>(1, expression);
            Assert.AreEqual("Name", mapping.Name);
        }
    }
}