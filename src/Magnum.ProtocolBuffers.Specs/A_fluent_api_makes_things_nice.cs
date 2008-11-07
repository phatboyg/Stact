namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;

    [TestFixture]
    public class A_fluent_api_makes_things_nice :
        Specification
    {
        [Test]
        public void Lets_map_SearchRequest()
        {
            var map = new MessageMap<SearchRequest>();
            map.Field(m => m.Query).MakeRequired();
            map.Field(m => m.PageNumber);
            map.Field(m => m.ResultPerPage);

            Assert.AreEqual(3, map.FieldCount);
        }
    }

    public class SearchRequest
    {
        public string Query { get; set; }
        public int? PageNumber { get; set; }
        public int? ResultPerPage { get; set; }
    }
}