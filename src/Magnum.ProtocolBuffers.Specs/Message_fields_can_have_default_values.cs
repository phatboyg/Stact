namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;

    [TestFixture]
    public class Message_fields_can_have_default_values
    {
        [Test]
        public void Should_be_settable()
        {
            var mapping = new FieldMapping<TestMessage, string>(m => m.Name, 1);
            mapping.SetDefaultValue("chris");

            Assert.IsTrue(mapping.HasDefaultValue);
            Assert.AreEqual("chris", mapping.DefaultValue);
        }

        [Test]
        public void By_default_there_should_be_no_default_value()
        {
            var mapping = new FieldMapping<TestMessage, string>(m => m.Name, 1);
            Assert.IsFalse(mapping.HasDefaultValue);
        }
    }
}