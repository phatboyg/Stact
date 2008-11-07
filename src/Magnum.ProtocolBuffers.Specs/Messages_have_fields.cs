namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Messages_have_fields
    {
        [Test]
        public void You_can_add_messages_fields()
        {
            var messageMap = new MessageMap<TestMessage>();
            messageMap.Field(m=>m.Name);
        }

        [Test]
        public void Should_be_able_to_add_more_than_one()
        {
            var messageMap = new MessageMap<TestMessage>();
            messageMap.Field(m => m.Name);
            messageMap.Field(m => m.Numbers);
            Assert.AreEqual(2, messageMap.FieldCount);
        }

        [Test]
        public void If_you_add_a_field_with_a_specific_number_tag_it_jumps_the_next_number()
        {
            var messageMap = new MessageMap<TestMessage>();
            messageMap.Field(m => m.Age, 5);
            Assert.AreEqual(6, messageMap.CurrentNumberTag);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void You_cant_add_two_fields_with_the_same_number_tag()
        {
            var messageMap = new MessageMap<TestMessage>();
            messageMap.Field(m => m.Age, 1);
            messageMap.Field(m => m.Name, 1);
        }
        
    }
}