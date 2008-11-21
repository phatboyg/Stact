namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using Common;
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class Messages_can_set_aside_numbers_for_extensions :
        Specification
    {
        [Test]
        public void Its_a_range()
        {
            var map = new MessageMap<TestMessage>();
            map.SetAsideExtensions(50, 100);

            Range<int> ext = new Range<int>(50,100, true, true);

            //map.ExtensionRange
            //    .ShouldEqual(ext);
        }

        [Test]
        [ExpectedException(typeof(ProtoMappingException))]
        public void You_cant_add_fields_in_the_extension_range()
        {
            var map = new MessageMap<TestMessage>();
            map.SetAsideExtensions(1,3);
            map.Field(m=>m.Name,2);
        }
    }
}