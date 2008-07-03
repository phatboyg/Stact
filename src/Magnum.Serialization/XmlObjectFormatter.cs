namespace Magnum.Serialization
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using System.Xml;

	public class XmlObjectFormatter : IObjectFormatter
	{
		private readonly MemoryStream _stream;
		private readonly XmlWriter _writer;

		public XmlObjectFormatter()
		{
			_stream = new MemoryStream();
			_writer = XmlWriter.Create(_stream);
		}

		public string GetString()
		{
			_writer.Flush();

			return Encoding.UTF8.GetString(_stream.ToArray());
		}

		public void Start()
		{
			_writer.WriteStartDocument();
		}

		public void Stop()
		{
			_writer.WriteEndDocument();
		}

		public void StartObject(Type type)
		{
			_writer.WriteStartElement("object");
			_writer.WriteAttributeString("type", type.FullName);
		}

		public void EndObject(Type type)
		{
			_writer.WriteEndElement();
		}

		public void WriteField(FieldInfo info, string value)
		{
			_writer.WriteElementString(info.Name, value);
		}

		public void WriteString(FieldInfo info, string value)
		{
			_writer.WriteElementString("string", value);
		}

		public byte[] ToArray()
		{
			_writer.Flush();
			return _stream.ToArray();
		}

		public void Dispose()
		{
			if (_stream != null)
			{
				_stream.Dispose();
			}
		}
	}
}