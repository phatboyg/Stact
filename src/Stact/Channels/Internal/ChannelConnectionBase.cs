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
namespace Stact.Internal
{
	using System;
	using System.Collections.Generic;
	using Magnum;
	using Magnum.Extensions;


	public abstract class ChannelConnectionBase
	{
		readonly HashSet<Channel> _connectedChannels;
		readonly Action<IEnumerable<Channel>> _disconnect;
		readonly HashSet<IDisposable> _disposables;
		bool _disposed;

		protected ChannelConnectionBase(Action<IEnumerable<Channel>> disconnect)
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
					catch
					{
					}
				});

			_disposables.Clear();
		}

		~ChannelConnectionBase()
		{
			Dispose(false);
		}
	}
}