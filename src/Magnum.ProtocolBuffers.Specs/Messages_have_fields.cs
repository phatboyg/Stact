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

        [Test]
        public void If_you_add_a_field_with_a_specific_number_tag_it_jumps_the_next_number()
        {
            var prop = ReflectionHelper.GetProperty<TestMessage, string>(m => m.Name);
            var messageMap = new MessageMap<TestMessage>();
            messageMap.Field(m => m.Age, 5);
            Assert.AreEqual(6, messageMap.CurrentNumberTag);
        }
        
    }
}