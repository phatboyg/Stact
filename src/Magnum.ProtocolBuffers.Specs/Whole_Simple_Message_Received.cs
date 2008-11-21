namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using NUnit.Framework;
    using ProtocolBuffers.Mapping;
    using ProtocolBuffers.Serialization;
    using ProtocolBuffers.Serialization.Streams;
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
            var desc = new MessageSerializerFactory().Build(map);
            var stream = new CodedOutputStream();

                desc.Serialize(stream, sr);


            //Assert.AreEqual(12, stream.Length);

            var codedStream = new CodedInputStream(stream.GetBytes());
            SearchRequest sr2 = (SearchRequest) desc.Deserialize(codedStream);

            Assert.AreEqual(sr.PageNumber, sr2.PageNumber);
            Assert.AreEqual(sr.ResultPerPage, sr2.ResultPerPage);
            Assert.AreEqual(sr.Query, sr2.Query);
        }

        [Test]
        public  void Serialize_Perf()
        {
            var sr = new SearchRequest()
            {
                PageNumber = 2,
                Query = "from q",
                ResultPerPage = 2
            };
            var map = new SearchRequestMapping();
            var desc = new MessageSerializerFactory().Build(map);
            var stream = new CodedOutputStream();

            for (int i = 0; i < 100000; i++)
            {
                desc.Serialize(stream, sr);
                
            }
        }

        [Test]
        public void Deserialize_Perf()
        {
            var sr = new SearchRequest()
            {
                PageNumber = 2,
                Query = "from q",
                ResultPerPage = 2
            };
            var map = new SearchRequestMapping();
            var desc = new MessageSerializerFactory().Build(map);
            var stream = new CodedOutputStream();

                desc.Serialize(stream, sr);
            byte[] bytes = stream.GetBytes();

            for (int i = 0; i < 100000; i++)
            {
                var codedStream = new CodedInputStream(bytes);
                desc.Deserialize(codedStream);
            }
        }

        [Test]
        public void Serialize_a_bigger_message()
        {
            var fedId = new Guid("90D8E35F-463C-4997-948B-A098ECC80854");
            var map = new MessageMap<TestMessage>();
            map.Field(m => m.IsCool);

            var msg = new TestMessage
                          {
                              Age = 1,
                              BirthDay = new DateTime(1979,2,26),
                              FederalIdNumber = fedId,
                              Name = "dru",
                              IsCool = true
                          };

            var desc = new MessageSerializerFactory().Build(map);
            var outStream = new CodedOutputStream();

            desc.Serialize(outStream, msg);

            //Assert.AreEqual(48, outStream.Length);

            var inStream = new CodedInputStream(outStream.GetBytes());
            var msg2 = (TestMessage)desc.Deserialize(inStream);

            Assert.AreEqual(msg.IsCool, msg2.IsCool);
            
        }

        [Test]
        public void Serialize_a_repeated_field()
        {
            var map = new MessageMap<TestMessage>();
            map.Field(m => m.Numbers);

            var msg = new TestMessage
            {
                Numbers = {1,2}
            };

            var desc = new MessageSerializerFactory().Build(map);
            var outStream = new CodedOutputStream();

            desc.Serialize(outStream, msg);

            Assert.AreEqual(6, outStream.Length);

            //var inStream = new CodedInputStream(outStream.GetBytes());
            //var msg2 = (TestMessage)desc.Deserialize(inStream);

            //Assert.AreEqual(msg.IsCool, msg2.IsCool);

        }
        
    }
}