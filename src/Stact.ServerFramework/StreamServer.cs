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
namespace Stact.ServerFramework
{
	using System;
	using System.Threading;
	using Events;


	public abstract class StreamServer
	{
		readonly Uri _uri;
		protected bool _closing;
		int _connectionCount;

		protected StreamServer(Uri uri, Fiber fiber, UntypedChannel eventChannel)
		{
			_closing = false;
			_uri = uri;
			Fiber = fiber;
			EventChannel = eventChannel;
		}

		public Uri BaseUri
		{
			get { return _uri; }
		}

		protected UntypedChannel EventChannel { get; private set; }
		protected Fiber Fiber { get; private set; }

		public int ConnectionCount
		{
			get { return _connectionCount; }
		}

		public void Publish<T>()
			where T : ServerEvent, new()
		{
			EventChannel.Send(new T());
		}

		public void Start()
		{
			Publish<ServerStarting>();
			StartListener(_uri);
			Publish<ServerRunning>();
		}

		public void Stop()
		{
			Publish<ServerStopping>();

			_closing = true;

			if (_connectionCount == 0)
				ShutdownListener();
		}

		protected virtual void StartListener(Uri uri) {}

		protected void ConnectionEstablished(Action handleConnectionAction)
		{
			Interlocked.Increment(ref _connectionCount);

			Fiber.Add(handleConnectionAction);
		}

		protected void ConnectionComplete()
		{
			int count = Interlocked.Decrement(ref _connectionCount);

			if (_closing)
			{
				if (count == 0)
					ShutdownListener();
			}
		}

		protected virtual void ShutdownListener()
		{
			Publish<ServerStopped>();
		}
	}
}