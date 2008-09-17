namespace Magnum.Serialization
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;

	public class BEncodeObjectFormatter : IObjectFormatter
	{
		private readonly Stream _stream;

		public BEncodeObjectFormatter(Stream stream)
		{
			_stream = stream;
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}

		public void StartObject(Type type)
		{
			_stream.WriteByte((byte) 'o');

			WriteString(type.AssemblyQualifiedName);

			_stream.WriteByte((byte)'d');
		}

		public void EndObject(Type type)
		{
			_stream.WriteByte((byte)'e');
			_stream.WriteByte((byte)'e');
			_stream.Flush();
		}

		public void Write(IPropertyData data)
		{
			throw new System.NotImplementedException();
		}

		public void WriteField(FieldInfo info, string value)
		{
			WriteString(info.Name);
			if(info.FieldType == typeof(int) ||
			   info.FieldType == typeof(long) ||
			   info.FieldType == typeof(Int32) ||
			   info.FieldType == typeof(Int64))
			{
				WriteNumber(long.Parse(value));
			}
			else
			{
				WriteString(value);
			}
		}

		public void WriteString(FieldInfo info, string value)
		{
			WriteString(info.Name);
			WriteString(value);
		}

		public void Dispose()
		{
		}

		private void WriteString(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				_stream.WriteByte((byte)'0');
				_stream.WriteByte((byte)':');
			}
			else
			{
				byte[] content = Encoding.UTF8.GetBytes(value);

				string s = string.Format("{0}:", content.Length);
				
				byte[] open = Encoding.ASCII.GetBytes(s);

				_stream.Write(open, 0, open.Length);
				_stream.Write(content, 0, content.Length);
			}
		}

		private void WriteNumber(long value)
		{
			_stream.WriteByte((byte)'i');

			byte[] content = Encoding.ASCII.GetBytes(value.ToString());
			_stream.Write(content, 0, content.Length);

			_stream.WriteByte((byte)'e');
		}
	}
}