namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using MbUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Message_fields_have_different_types :
        Specification
    {
        [Test]
        public void Fields_with_reference_types_should_be_optional_by_default()
        {
            var mapping = new FieldMap<TestMessage>( 1, m=>m.Name);
            mapping.Rules
                .ShouldEqual(FieldRules.Optional);
        }

        [Test]
        public void Field_should_self_set_repeated()
        {
            var mapping = new FieldMap<TestMessage>(1, m=>m.Numbers);
            mapping.Rules
                .ShouldEqual(FieldRules.Repeated);
        }

        [Test]
        public void Field_should_self_set_required()
        {
            var mapping = new FieldMap<TestMessage>(1, m=>m.Age);
            mapping.Rules
                .ShouldEqual(FieldRules.Required);

            var mapping2 = new FieldMap<TestMessage>(1, m => m.BirthDay);
            mapping2.Rules
                .ShouldEqual(FieldRules.Required);
        }

        [Test]
        public void Nullable_fields_should_be_optional_by_default()
        {
            var mapping = new FieldMap<TestMessage>(1, m=>m.NumberOfPets);
            mapping.Rules
                .ShouldEqual(FieldRules.Optional);

            var mapping2 = new FieldMap<TestMessage>(1, m => m.DeadDay);
            mapping2.Rules
                .ShouldEqual(FieldRules.Optional);
        }
    }
}