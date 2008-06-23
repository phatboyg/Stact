namespace Magnum.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading;

	public class BDecode : IDisposable
	{
		private readonly Stream _inbound;
		private readonly Queue<object> _output = new Queue<object>();
		private Thread _thread;
		private BinaryReader _reader;
		private readonly ManualResetEvent _killThread = new ManualResetEvent(false);
		private readonly Semaphore _outputLength = new Semaphore(0, int.MaxValue);


		public BDecode(Stream inbound)
		{
			_inbound = inbound;

			_thread = new Thread(DecoderThread);
			_thread.IsBackground = true;
			_thread.Start();
		}

		public object Read(TimeSpan timeout)
		{
			if(_outputLength.WaitOne(timeout, true))
			{
				lock(_output)
				{
					return _output.Dequeue();
				}
			}

			return null;
		}

		private void DecoderThread()
		{
			_reader = new BinaryReader(_inbound);
			using (_reader)
			{
				while (_killThread.WaitOne(0, true) == false)
				{
					object obj = ReadObject();

					lock (_output)
						_output.Enqueue(obj);
				}
			}
		}

		private object ReadObject()
		{
			char b = _reader.ReadChar();
			switch (b)
			{
				case 'i':
					return ReadLong();

				case 'l':
					return ReadList();

				case 'd':
					return ReadDictionary();

				default:
					int digit;
					if(int.TryParse(b.ToString(), out digit))
					{
						string number = b.ToString();
						while ( int.TryParse((b = _reader.ReadChar()).ToString(), out digit))
						{
							number += b;
						}

						int length;
						int.TryParse(number, out length);

						byte[] data = new byte[length];

						_reader.Read(data, 0, length);

						return data;
					}
					else 
						throw new ApplicationException("Wow, something bogus in the stream");
			}
		}

		private Dictionary<object,object> ReadDictionary()
		{
			Dictionary<object,object> dictionary = new Dictionary<object, object>();
			while ((_reader.PeekChar()) != 'e')
			{
				object key = ReadObject();
				object value = ReadObject();

				dictionary.Add(key, value);
			}

			return dictionary;
		}

		private List<object> ReadList()
		{
			List<object> objects = new List<object>();
			while ( (_reader.PeekChar()) != 'e')
			{
				objects.Add(ReadObject());

				
			}

			return objects;
		}

		private long ReadLong()
		{
			string number = "";

			int b;
			while ( ( b = _reader.Read() ) != 'e' )
				number += b;

			return long.Parse(number);
		}

		public void Dispose()
		{
			_killThread.Set();
			_thread.Join(TimeSpan.FromSeconds(10));
			
		}
	}
}