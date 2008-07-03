namespace Magnum.Transport.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using Serialization;

	public class PeerConnection : IConnection
	{
		private readonly BDecode _decoder;
		private readonly Action<PeerConnection> _disposeCallback;
		private readonly EndPoint _endpoint;
		private readonly IObjectFormatter _formatter;
		private readonly IObjectSerializer _serializer;
		private readonly Socket _socket;
		private readonly SocketStream _stream;

		public PeerConnection(Socket socket, EndPoint endpoint, Action<PeerConnection> disposeCallback)
		{
			_socket = socket;
			_stream = new SocketStream(socket);
			_decoder = new BDecode(_stream);

			_formatter = new BEncodeObjectFormatter();
			_serializer = new BasicObjectSerializer(_formatter);

			_endpoint = endpoint;
			_disposeCallback = disposeCallback;
		}

		public EndPoint Endpoint
		{
			get { return _endpoint; }
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);

				_decoder.Dispose();
				_stream.Dispose();

				// can't dispose a socket for some reason, so this should handle it
				using (_socket)
					_socket.Shutdown(SocketShutdown.Both);
			}
			else
			{
				Action<PeerConnection> disposeCallback = _disposeCallback;
				if (disposeCallback != null)
				{
					disposeCallback(this);
				}
			}
		}

		public void Send<T>(T message) where T : class
		{
			using (IObjectFormatter formatter = new BEncodeObjectFormatter())
			using (IObjectSerializer serializer = new BasicObjectSerializer(formatter))
			{
				serializer.Serialize(message);

				byte[] blob = formatter.ToArray();

				//byte[] open = Encoding.UTF8.GetBytes(string.Format("{0}:", blob.Length));

				//List<ArraySegment<byte>> bytes = new List<ArraySegment<byte>>(open.Length + blob.Length);

				//bytes.Add(new ArraySegment<byte>(open));
				//bytes.Add(new ArraySegment<byte>(blob));

				Debug.WriteLine("WRITE: " + Encoding.UTF8.GetString(blob));

				_socket.Send(blob);
			}
		}

		public object Receive(TimeSpan timeout)
		{
			object obj = _decoder.Read(timeout);

			return obj;
		}
	}

	public class SocketStream : Stream
	{
		private readonly Socket _socket;

		public SocketStream(Socket socket)
		{
			_socket = socket;
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			SocketError errorCode;

			int retval = _socket.Receive(buffer, offset, count, SocketFlags.None, out errorCode);

			if (errorCode == SocketError.Success)
				return retval;

			throw new IOException(String.Format("Failed to read from the socket '{0}'. Error: {1}", _socket.RemoteEndPoint, errorCode));
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}
	}
}