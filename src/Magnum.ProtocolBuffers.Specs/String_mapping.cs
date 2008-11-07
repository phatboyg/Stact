namespace Magnum.ProtocolBuffers.Specs
{
    using System;
    using System.Text;
    using Internal;
    using NUnit.Framework;
    using TestMessages;

    [TestFixture]
    public class String_mapping
    {
        [Test]
        public void Does_it_match()
        {
            var map = new MessageMap<SearchRequest>();
            map.Field(m=>m.Query).MakeRequired();
            map.Field(m => m.PageNumber);
            map.Field(m=>m.ResultPerPage).SetDefaultValue(10);

            IMapping mm =  map;
            StringVisitor v = new StringVisitor();
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

    internal class StringVisitor : 
        IMappingVisitor
    {
        StringBuilder sb = new StringBuilder();

        public Type CurrentType
        {
            get { return null; }
            set {  }
        }

        public void AddMap(string contentToAppend)
        {
            sb.AppendLine(contentToAppend);
        }

        public void AddSerializer(Type typeMapped, object serializer)
        {
            throw new System.NotImplementedException();
        }

        public string GetMap()
        {
            return sb.ToString();
        }
    }
}