namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;

    [TestFixture]
    public class Message_fields_have_names
    {
        [Test]
        public void Name_should_be_set_automatically()
        {
            var mapping = new FieldMapping<TestMessage, string>(m => m.Name, 1);
            Assert.AreEqual("Name", mapping.FieldName);
        }
    }
}