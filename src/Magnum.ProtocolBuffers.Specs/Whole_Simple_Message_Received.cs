namespace Magnum.ProtocolBuffers.Specs
{
    using System;
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
            
            //Assert.AreEqual(12, stream.Length);

            var codedStream = new CodedInputStream(stream.GetBytes());
            var sr2 = desc.Deserialize(codedStream);

            Assert.AreEqual(sr.PageNumber, sr2.PageNumber);
            Assert.AreEqual(sr.ResultPerPage, sr2.ResultPerPage);
            Assert.AreEqual(sr.Query, sr2.Query);
        }

        [Test]
        public void Serialize_a_bigger_message()
        {
            var fedId = new Guid("90D8E35F-463C-4997-948B-A098ECC80854");
            var map = new MessageMap<TestMessage>();
            map.Field(m => m.BirthDay);
            map.Field(m => m.DeadDay);
            map.Field(m => m.FederalIdNumber);

            var msg = new TestMessage
                          {
                              Age = 1,
                              BirthDay = new DateTime(1979,2,26),
                              FederalIdNumber = fedId,
                              Name = "dru"
                          };

            var desc = new MessageDescriptorFactory().Build(map);
            var outStream = new CodedOutputStream();

            desc.Serialize(outStream, msg);

            //Assert.AreEqual(48, outStream.Length);

            var inStream = new CodedInputStream(outStream.GetBytes());
            var msg2 = desc.Deserialize(inStream);

            Assert.AreEqual(msg.FederalIdNumber, msg2.FederalIdNumber);
            Assert.AreEqual(msg.BirthDay, msg2.BirthDay);
        }
        
    }
}