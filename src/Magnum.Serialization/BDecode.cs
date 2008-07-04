namespace Magnum.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Text;
	using System.Threading;

	public class BDecode : IDisposable
	{
		private readonly Stream _stream;
		private readonly Queue<object> _output = new Queue<object>();
		private readonly Thread _thread;
		private readonly ManualResetEvent _killThread = new ManualResetEvent(false);
		private readonly Semaphore _outputLength = new Semaphore(0, int.MaxValue);


		public BDecode(Stream stream)
		{
			_stream = stream;

			_thread = new Thread(DecoderThread);
			_thread.IsBackground = true;
			_thread.Start();
		}

		public object Read(TimeSpan timeout)
		{
			if (_outputLength.WaitOne(timeout, true))
			{
				lock (_output)
				{
					return _output.Dequeue();
				}
			}

			return null;
		}

		private void DecoderThread()
		{
			try
			{
				while (!Killed)
				{
					object obj = ReadObject();
					if (obj != null)
					{
						lock (_output)
							_output.Enqueue(obj);

						_outputLength.Release();
					}
				}

				Debug.WriteLine("Exiting Decoder Thread");
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Decoder Thread Threw an Exception: " + ex.Message);
			}
		}

		private bool Killed
		{
			get { return _killThread.WaitOne(0, false); }
		}

		private object ReadObject()
		{
			int i = _stream.ReadByte();
			if(i == -1)
			{
				_killThread.Set();
				return null;
			}

			char b = (char)i;
			switch (b)
			{
				case 'e':
					return null;

				case 'o':
					return ReadTypedObject();

				case 'i':
					return ReadLong();

				case 'l':
					return ReadList();

				case 'd':
					return ReadDictionary();

				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':

					// TODO overflow possible
					string number = "" + b;

					while (IsDigit((b = (char) _stream.ReadByte())))
					{
						number += b;
					}

					int length;
					int.TryParse(number, out length);

					byte[] data = new byte[length];

					_stream.Read(data, 0, length);

					return data;

				default:
					throw new ApplicationException("Wow, something bogus in the stream");
			}
		}

		private static bool IsDigit(char c)
		{
			switch(c)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					return true;

				default:
					return false;
			}
		}

		private object ReadTypedObject()
		{
			string typeName = ReadString();

			Dictionary<object, object> fields = ReadObject() as Dictionary<object, object>;

			_stream.ReadByte(); // get rid of the 'e'

			Type t = Type.GetType(typeName);

			object o = FormatterServices.GetSafeUninitializedObject(t);

			MemberInfo[] memberInfo = FormatterServices.GetSerializableMembers(t);

			object[] values = new object[memberInfo.Length];

			Dictionary<object, object>.ValueCollection.Enumerator fieldValues = fields.Values.GetEnumerator();

			for (int index = 0; index < memberInfo.Length; index++)
			{
				fieldValues.MoveNext();

				if (memberInfo[index].MemberType == MemberTypes.Field)
				{
					FieldInfo fieldInfo = (FieldInfo) memberInfo[index];

					TypeConverter tc = TypeDescriptor.GetConverter(fieldInfo.FieldType);

					if(tc.CanConvertFrom(fieldValues.Current.GetType()))
						values[index] = tc.ConvertFrom(fieldValues.Current);
					else
					{
						TypeConverter tcv = TypeDescriptor.GetConverter(fieldValues.Current.GetType());
						object value = tcv.ConvertTo(fieldValues.Current, typeof (string));

						values[index] = tc.ConvertFrom(value);
					}
				}
			}

			FormatterServices.PopulateObjectMembers(o, memberInfo, values);

			return o;
		}

		private Dictionary<object, object> ReadDictionary()
		{
			Dictionary<object, object> dictionary = new Dictionary<object, object>();
			object obj;
			while ((obj = ReadObject()) != null)
			{
				string key = Encoding.UTF8.GetString((byte[]) obj);

				object value = ReadObject();
				if (value.GetType() == typeof (byte[]))
					value = Encoding.UTF8.GetString((byte[]) value);

				dictionary.Add(key, value);
			}

			return dictionary;
		}

		private string ReadString()
		{
			string s = Encoding.UTF8.GetString(ReadObject() as byte[]);

			return s;
		}

		private List<object> ReadList()
		{
			List<object> objects = new List<object>();

			object obj;
			while ((obj = ReadObject()) != null)
			{
				objects.Add(obj);
			}

			return objects;
		}

		private long ReadLong()
		{
			string number = "";

			char b;
			while ((b = (char)_stream.ReadByte()) != 'e')
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