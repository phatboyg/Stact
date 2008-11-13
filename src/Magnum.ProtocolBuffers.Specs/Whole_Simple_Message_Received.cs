namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;
    using ProtocolBuffers.Serialization;
    using TestMappings;
    using TestMessages;

    [TestFixture]
    public class Whole_Simple_Message_Received :
        Specification
    {
        [Test]
        public void Serialize_Simple_message()
        {
            var sr = new SearchRequest()
                         {
                             PageNumber = 2,
                             Query = "from q",
                             ResultPerPage = 2
                         };
            var map = new SearchRequestMapping();
            var desc = new MessageDescriptorFactory().Build(map);
            var stream = new CodedOutputStream();

            desc.Serialize(stream, sr);
            
            Assert.AreEqual(3, stream.Length);
        }
        
    }
}