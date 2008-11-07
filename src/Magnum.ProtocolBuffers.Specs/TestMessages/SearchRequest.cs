namespace Magnum.ProtocolBuffers.Specs.TestMessages
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public int? PageNumber { get; set; }
        public int? ResultPerPage { get; set; }
    }
}