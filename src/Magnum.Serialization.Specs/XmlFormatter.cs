using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Magnum.Serialization.Specs
{
    internal class XmlFormatter : ISerializationFormatter
    {
        private readonly XmlWriter _writer;
        private readonly MemoryStream _stream;

        public XmlFormatter()
        {
            _stream = new MemoryStream();
            _writer = XmlWriter.Create(_stream);

            _writer.WriteStartDocument();
        }

        public void StartObject(Type objectType)
        {
            _writer.WriteStartElement(objectType.Name);
            _writer.WriteAttributeString("type", objectType.FullName);
        }

        public string GetString()
        {
            _writer.WriteEndDocument();
            _writer.Flush();

            return Encoding.UTF8.GetString(_stream.ToArray());
        }

        public void SetProperty(string name, Type type, object value)
        {
            _writer.WriteStartElement(name);
            //_writer.WriteAttributeString("type", type.FullName);
            _writer.WriteValue(value);
            _writer.WriteEndElement();
        }
    }
}