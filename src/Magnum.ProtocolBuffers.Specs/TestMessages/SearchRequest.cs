namespace Magnum.ProtocolBuffers.Specs
{
    public class SearchRequest
    {
        public string Query { get; set; }
        public int? PageNumber { get; set; }
        public int? ResultPerPage { get; set; }
    }
}