namespace Magnum.ProtocolBuffers.Specs.TestMappings
{
    using TestMessages;

    public class SearchRequestMapping :
        MessageMap<SearchRequest>
    {
        public SearchRequestMapping()
        {
            Field(m=>m.Query).MakeRequired();
            Field(m => m.PageNumber);
            Field(m=>m.ResultPerPage).SetDefaultValue(10);
        }
    }
}