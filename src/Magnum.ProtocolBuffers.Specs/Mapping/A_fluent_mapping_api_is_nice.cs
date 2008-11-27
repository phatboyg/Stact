namespace Magnum.ProtocolBuffers.Specs.Mapping
{
    using MbUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class A_fluent_mapping_api_is_nice :
        Specification
    {
        [Test]
        public void Lets_map_SearchRequest()
        {
            var map = new MessageMap<SearchRequest>();
            map.Field(m => m.Query).MakeRequired();
            map.Field(m => m.PageNumber);
            map.Field(m => m.ResultPerPage).SetDefaultValue(10);
        }
    }
}