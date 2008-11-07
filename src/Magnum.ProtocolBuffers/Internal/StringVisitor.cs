namespace Magnum.ProtocolBuffers.Internal
{
    using System;
    using System.Text;

    public class StringVisitor : 
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