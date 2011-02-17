namespace Stact.ServerFramework.Specs
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Text;
	using Internal;
	using Magnum.Serialization;
	using Magnum.TestFramework;
	using Stact.Internal;


	[Scenario]
	public class When_serializing_a_message_to_the_wire
	{

		[When]
		public void Serializing_a_message_to_the_wire()
		{
			var a = new AImpl("bob", "usa");
			var b = new AImpl("joe", "america");

			IDictionary<string,string> headers = new Dictionary<string, string>();

			headers.Add("SourceAddress", "pgm://234.0.0.7:9001/5D84D97B-4617-40AF-893B-A2274AD3157F");
			headers.Add("DestinationAddress", "pgm://234.0.0.7:9000/");

			var serializer = new FastTextSerializer();
			byte[] empty = new byte[0];


			ArraySegment<byte> message = new ArraySegment<byte>(empty, 0, 0);

			for (int i = 0; i < 10; i++)
			{
				message = CreateMessage(headers, serializer, a, b);
			}

			Stopwatch timer = Stopwatch.StartNew();
			const int loops = 10000;
			for (int i = 0; i < loops; i++)
			{
				message = CreateMessage(headers, serializer, a, b);
			}
			timer.Stop();

			Trace.WriteLine("Messages/sec: " + loops * 2 * 1000 / timer.ElapsedMilliseconds);

			DumpBuffer(message.Array, message.Offset, message.Count);
		}

		ArraySegment<byte> CreateMessage(IDictionary<string, string> headers, FastTextSerializer serializer, AImpl a, AImpl b)
		{
			ArraySegment<byte> message;
			byte[] padding = new byte[8];
			using (var ms = new MemoryStream(4096))
			{
				using (var ts = new StreamWriter(ms.PreventClose()))
				{
					int msOffset = 0;

					WriteMessageToBuffer(headers, ms,ts, padding, msOffset, serializer, a);

					msOffset = (int)ms.Length;

					WriteMessageToBuffer(headers, ms,ts, padding, msOffset, serializer, b);

					message = new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
				}
			}
			return message;
		}

		void WriteMessageToBuffer(IDictionary<string, string> headers, MemoryStream ms, TextWriter ts, byte[] padding, int msOffset, FastTextSerializer serializer, AImpl a)
		{
			ms.Write(padding, 0, 8);

				serializer.Serialize(headers, ts);
				ts.Flush();

				int headerLength = (int)ms.Length - msOffset - 8;

				serializer.Serialize(a, ts);
				ts.Flush();

				int bodyLength = (int)ms.Length - msOffset - headerLength - 8;

				var buffer = ms.GetBuffer();

				new WireMessageHeader(headerLength, bodyLength)
					.WriteToBuffer(buffer, msOffset, 8);
		}

		void DumpBuffer(byte[] buffer, int offset, int length)
		{
			for (int i = offset; i < length;)
			{
				string printable = "  ";

				int j = 0;
				for (; j < 16 && i < length; j++, i++)
				{
					Trace.Write(buffer[i].ToString("X2") + " ");
					if (buffer[i] >= 0x20 && buffer[i] <= 0x7F)
						printable = printable + Convert.ToChar(buffer[i]);
					else
					{
						printable = printable + ".";
					}
				}
				for (; j < 16; j++)
				{
					Trace.Write("   ");
				}

				Trace.WriteLine(printable);
			}
		}


		public interface A
		{
			string Name { get; }
			string Address { get; }
		}

		public class AImpl : A
		{
			public AImpl(string name, string address)
			{
				Name = name;
				Address = address;
			}

			public AImpl()
			{
			}

			public string Name { get; private set; }
			public string Address { get; private set; }
		}
	}
}