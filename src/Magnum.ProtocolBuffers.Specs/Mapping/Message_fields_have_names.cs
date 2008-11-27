namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using System;
    using System.Linq.Expressions;
    using MbUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Message_fields_have_names
    {
        [Test]
        public void Name_should_be_set_automatically()
        {
            Expression<Func<TestMessage, object>> expression = m => m.Name;
            var mapping = new FieldMap<TestMessage>(1, expression);

            mapping.Name
                .ShouldEqual("Name");
        }
    }
}