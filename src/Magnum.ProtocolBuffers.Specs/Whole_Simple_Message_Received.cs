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
            
            Assert.AreEqual(12, stream.Length);

            var codedStream = new CodedInputStream(stream.GetBytes());
            var sr2 = desc.Deserialize(codedStream);

            Assert.AreEqual(sr.PageNumber, sr2.PageNumber);
            Assert.AreEqual(sr.ResultPerPage, sr2.ResultPerPage);
            Assert.AreEqual(sr.Query, sr2.Query);
        }
        
    }
}