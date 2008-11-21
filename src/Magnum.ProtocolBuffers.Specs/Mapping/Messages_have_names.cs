namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using NUnit.Framework;
    using ProtocolBuffers.Mapping;
    using TestMessages;

    [TestFixture]
    public class Messages_have_names
    {
        [Test]
        public void Name_should_be_set_to_class_name()
        {
            var messageMapping = new MessageMap<TestMessage>();
            //messageMapping.Name
            //    .ShouldEqual("TestMessage");
        }

        [Test]
        public void Name_should_be_overridable()
        {
            var messageMapping = new MessageMap<TestMessage>();
            messageMapping.OverrideName("dru");
            //messageMapping.Name
            //    .ShouldEqual("dru");
        }
        
    }
}