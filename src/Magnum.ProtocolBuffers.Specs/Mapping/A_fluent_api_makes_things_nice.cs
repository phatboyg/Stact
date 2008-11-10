namespace Magnum.ProtocolBuffers.Specs
{
    using NUnit.Framework;
    using TestMessages;

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
            map.Field(m => m.ResultPerPage).SetDefaultValue(10);
            
            Assert.AreEqual(3, map.FieldCount);
        }
    }
}