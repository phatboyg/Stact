namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Messages_have_names
    {
        [Test]
        public void Name_should_be_set_to_class_name()
        {
            var messageMapping = new MessageMap<TestMessage>();
            Assert.AreEqual("TestMessage", messageMapping.Name);
        }
        
    }
}