namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;

    [TestFixture]
    public class Message_fields_can_have_default_values :
        Specification
    {
        private FieldMap _mapping;

        protected override void Before_each()
        {
            var prop = ReflectionHelper.GetProperty<TestMessage, string>(m => m.Name);
            _mapping = new FieldMap(prop, 1);
        }
        [Test]
        public void Should_be_settable()
        {
            
            _mapping.SetDefaultValue("chris");

            Assert.IsTrue(_mapping.HasDefaultValue);
            Assert.AreEqual("chris", _mapping.DefaultValue);
        }

        [Test]
        public void By_default_there_should_be_no_default_value()
        {
            Assert.IsFalse(_mapping.HasDefaultValue);
        }
    }
}