namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using MbUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Message_fields_can_have_default_values :
        Specification
    {
        private FieldMap<TestMessage> _mapping;

        protected override void Before_each()
        {
            _mapping = new FieldMap<TestMessage>(1, m=>m.Name);
        }
        [Test]
        public void Should_be_settable()
        {
            _mapping.SetDefaultValue("chris");

            _mapping.HasDefaultValue
                .ShouldBeTrue();
            
            _mapping.DefaultValue
                .ShouldEqual("chris");
        }

        [Test]
        public void By_default_there_should_be_no_default_value()
        {
            _mapping.HasDefaultValue
                .ShouldBeFalse();
        }
    }
}