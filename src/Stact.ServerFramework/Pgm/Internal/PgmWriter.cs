// Copyright 2010 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Stact.ServerFramework.Internal
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Net;
	using Magnum.Extensions;


	/// <summary>
	/// Sends messages via Pragmatic General Multicast
	/// </summary>
	public class PgmWriter :
		BlockWriter,
		IDisposable
	{
		readonly Uri _uri;
		bool _disposed;
		Uri _localUri;
		int _sendBufferSize;
		PgmSocket _socket;
		BlockWriter _socketWriter;

		public PgmWriter(Uri uri)
		{
			_sendBufferSize = 256*1024;
			_uri = uri;

			_socket = new PgmSocket();
			_socketWriter = new SocketWriter(_socket);
		}

		public Uri LocalUri
		{
			get { return _localUri; }
			set { _localUri = value; }
		}

		public int SendBufferSize
		{
			get { return _sendBufferSize; }
			set { _sendBufferSize = value; }
		}

		public void Write(ArraySegment<byte> block, Action<ArraySegment<byte>> unsentCallback)
		{
			_socketWriter.Write(block, unsentCallback);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Start()
		{
			_socket.SendBufferSize = _sendBufferSize;

			IPEndPoint endpoint = _uri.ResolveHostName().Single();
			IPEndPoint bindEndpoint = GetBindEndpoint();


			Trace.WriteLine("Binding producer to address: " + endpoint);
			_socket.Bind(bindEndpoint);

			_socket.EnableHighSpeed();

			Trace.WriteLine("Connecting producer to address: " + endpoint);
			_socket.Connect(endpoint);
		}

		IPEndPoint GetBindEndpoint()
		{
			return _localUri != null
			       	? _localUri.ResolveHostName().Single()
			       	: new IPEndPoint(IPAddress.Any, 0);
		}

		~PgmWriter()
		{
			Dispose(false);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
			{
				if (_socket != null)
				{
					_socket.Close();
					(_socket as IDisposable).Dispose();
					_socket = null;
				}
			}

			_disposed = true;
		}
	}
}