namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using MbUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Message_fields_have_assigned_number_tags :
        Specification
    {
        [Test]
        public void Mappings_need_number_tags()
        {
            var fieldMapping = new FieldMap<TestMessage>(1, m=>m.Name);
            fieldMapping.NumberTag
                .ShouldEqual(1);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void You_cant_use_19000()
        {
            new FieldMap<TestMessage>(19000,m=>m.DeadDay);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void You_cant_use_19999()
        {
            new FieldMap<TestMessage>(19999,m=>m.DeadDay);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void Or_anything_between()
        {
            new FieldMap<TestMessage>(19500, m=>m.DeadDay);
        }
    }
}