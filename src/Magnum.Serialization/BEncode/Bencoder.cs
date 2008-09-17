namespace Magnum.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	public class BEncoder : IDisposable
	{
		private readonly MemoryStream _stream;
		private readonly TextWriter _writer;

		public BEncoder()
		{
			_stream = new MemoryStream();
			_writer = new StreamWriter(_stream);
		}

		public void Dispose()
		{
			if (_writer != null)
				_writer.Dispose();

			if (_stream != null)
				_stream.Dispose();
		}

		public string GetString()
		{
			_writer.Flush();

			return Encoding.UTF8.GetString(_stream.ToArray());
		}

		public byte[] GetBytes()
		{
			_writer.Flush();

			return _stream.ToArray();
		}

		public MemoryStream GetStream()
		{
			return _stream;
		}

		public void Append(string value)
		{
			string s = value.Length.ToString() + ':' + value;

			_writer.Write(s);
		}

		public void Append(IEnumerable<string> value)
		{
			_writer.Write('l');

			foreach (string s in value)
			{
				Append(s);
			}

			_writer.Write('e');
		}

		public void Append(int value)
		{
			string s = "i" + value + "e";

			_writer.Write(s);
		}

		public void Append(long value)
		{
			string s = "i" + value + "e";

			_writer.Write(s);
		}

		public void AppendNull()
		{
			_writer.Write("n");
		}
	}
}