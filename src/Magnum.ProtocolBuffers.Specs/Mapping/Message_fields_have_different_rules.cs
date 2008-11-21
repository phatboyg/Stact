namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using NUnit.Framework;
    using TestMessages;

    public class Message_fields_have_different_rules :
        Specification
    {
        private FieldMap<TestMessage> _fieldMap;

        protected override void Before_each()
        {
            _fieldMap = new FieldMap<TestMessage>(1, m=>m.Name);
        }

        [Test]
        public void Mappings_are_optional_by_default()
        {
            _fieldMap.Rules
                .ShouldEqual(FieldRules.Optional);
        }

        [Test]
        public void Mappings_can_be_made_required()
        {
            _fieldMap.MakeRequired();


            _fieldMap.Rules
                .ShouldEqual(FieldRules.Required);
        }

        [Test]
        public void Mappings_can_be_made_repeated()
        {
            _fieldMap.MakeRepeated();

            _fieldMap.Rules
                .ShouldEqual(FieldRules.Repeated);
        }
    }
}