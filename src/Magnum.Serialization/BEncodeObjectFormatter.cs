namespace Magnum.Serialization
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Text;

	public class BEncodeObjectFormatter : IObjectFormatter
	{
		private readonly MemoryStream _stream;
		private readonly TextWriter _writer;

		public BEncodeObjectFormatter()
		{
			_stream = new MemoryStream();
			_writer = new StreamWriter(_stream);
		}

		public string GetString()
		{
			_writer.Flush();

			return Encoding.UTF8.GetString(_stream.ToArray());
		}

		public void Start()
		{
		}

		public void Stop()
		{
		}

		public void StartObject(Type type)
		{
			_writer.Write('o');
			WriteString(type.FullName);
			_writer.Write('d');
		}

		public void EndObject(Type type)
		{
			_writer.Write("ee");
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

		public byte[] ToArray()
		{
			_writer.Flush();
			return _stream.ToArray();
		}

		public void Dispose()
		{
			using(_stream)
			using (_writer)
			{
				_writer.Close();
				_stream.Close();
			}
		}

		private void WriteString(string value)
		{
			if (string.IsNullOrEmpty(value))
				_writer.Write("0:");
			else
			{
				string s = value.Length.ToString() + ':' + value;
				_writer.Write(s);
			}
		}

		private void WriteNumber(long value)
		{
			string s = "i" + value + "e";
			_writer.Write(s);
		}
	}
}