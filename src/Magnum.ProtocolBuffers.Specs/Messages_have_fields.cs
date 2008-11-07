namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;

    [TestFixture]
    public class Messages_have_fields
    {
        [Test]
        public void You_can_add_messages_fields()
        {
            var prop = ReflectionHelper.GetProperty<TestMessage, string>(m => m.Name);
            var messageMap = new MessageMap<TestMessage>();
            messageMap.AddField(new FieldMap(prop, 1));
        }

        [Test]
        public void Should_be_able_to_add_more_than_one()
        {
            var prop = ReflectionHelper.GetProperty<TestMessage, string>(m => m.Name);
            var messageMap = new MessageMap<TestMessage>();
            messageMap.AddField(new FieldMap(prop, 1));
            messageMap.AddField(new FieldMap(prop, 2));
            Assert.AreEqual(2, messageMap.FieldCount);
        }
        
    }
}