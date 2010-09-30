// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Stact.Channels.Configuration.Internal
{
	using System;
	using System.Collections.Generic;
	using Extensions;
	using Logging;


	public abstract class AbstractChannelConnection
	{
		readonly Action<IEnumerable<Channel>> _disconnect;
		static readonly ILogger _log = Logger.GetLogger<AbstractChannelConnection>();
		readonly HashSet<Channel> _connectedChannels;
		readonly HashSet<IDisposable> _disposables;
		bool _disposed;

		protected AbstractChannelConnection(Action<IEnumerable<Channel>> disconnect)
		{
			Guard.AgainstNull(disconnect);

			_disconnect = disconnect;

			_connectedChannels = new HashSet<Channel>();
			_disposables = new HashSet<IDisposable>();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Disconnect()
		{
			if (_connectedChannels.Count == 0)
				return;

			_disconnect(_connectedChannels);

			_connectedChannels.Clear();
		}

		public void AddChannel(Channel channel)
		{
			_connectedChannels.Add(channel);
		}

		public void AddDisposable(IDisposable disposable)
		{
			_disposables.Add(disposable);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
			{
				Disconnect();

				DisposeOfDisposables();
			}

			_disposed = true;
		}

		void DisposeOfDisposables()
		{
			_disposables.Each(x =>
				{
					try
					{
						x.Dispose();
					}
					catch (Exception ex)
					{
						_log.Error(l => l.Write(ex, "An exception occurred disposing of a connection object"));
					}
				});

			_disposables.Clear();
		}

		~AbstractChannelConnection()
		{
			Dispose(false);
		}
	}
}