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
namespace Magnum.Channels
{
	using System;
	using System.ServiceModel;
	using Fibers;

	public class LocalWcfChannelServiceHost<T> :
		IDisposable
	{
		private readonly string _pipeName;
		private readonly LocalWcfChannelService<T> _service;
		private readonly Uri _serviceUri;
		private bool _disposed;
		private ServiceHost _serviceHost;

		public LocalWcfChannelServiceHost(Fiber fiber, Uri serviceUri, string pipeName, Channel<T> output)
		{
			_serviceUri = serviceUri;
			_pipeName = pipeName;

			_service = new LocalWcfChannelService<T>(fiber, output);

			_serviceHost = new ServiceHost(_service, _serviceUri);
			_serviceHost.AddServiceEndpoint(typeof (LocalWcfChannel<T>), new NetNamedPipeBinding(), _pipeName);
			_serviceHost.Open();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Stop()
		{
			if ((_serviceHost != null) && (_serviceHost.State != CommunicationState.Closed))
			{
				_serviceHost.Close();
				_serviceHost = null;
			}
		}

		private void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				Stop();
			}

			_disposed = true;
		}

		~LocalWcfChannelServiceHost()
		{
			Dispose(false);
		}
	}
}