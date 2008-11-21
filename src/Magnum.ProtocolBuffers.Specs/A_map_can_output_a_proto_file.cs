namespace Magnum.ProtocolBuffers.Specs
{
    using Internal;
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class A_map_can_output_a_proto_file
    {
        [Test]
        public void Does_it_match()
        {
            var map = new MessageMap<SearchRequest>();
            map.Field(m=>m.Query).MakeRequired();
            map.Field(m => m.PageNumber);
            map.Field(m=>m.ResultPerPage).SetDefaultValue(10);

            IMap mm =  map;
            var v = new StringVisitor();
            mm.Visit(v);

            string proto = @"message SearchRequest {
  required string query = 1;
  optional int32 page_number = 2;
  optional int32 result_per_page = 3 [default = 10];
}
";
            Assert.AreEqual(proto, v.GetMap());
        }
        
    }
}