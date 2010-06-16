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
	using Internal;
	using Serialization;

	/// <summary>
	/// Receives messages from a named pipe via WCF and forwards them to the specific channel. Messages
	/// are first serialized to a wire type that is already WCF compliant, making it unnecessary to decorate
	/// your message objects with WCF data contract serializer attributes.
	/// </summary>
	public class WcfChannelHost :
		IDisposable
	{
		private readonly WcfChannelService<WcfMessageEnvelope> _service;
		private readonly Uri _serviceUri;

		private bool _disposed;
		private ServiceHost _serviceHost;

		public WcfChannelHost(Fiber fiber, UntypedChannel output, Uri serviceUri, string pipeName)
		{
			_serviceUri = serviceUri;

			var channel = new DeserializeMessageEnvelopeChannel<WcfMessageEnvelope>(fiber, new FastTextSerializer(), output);

			_service = new WcfChannelService<WcfMessageEnvelope>(channel);

			_serviceHost = new ServiceHost(_service, _serviceUri);
			_serviceHost.AddServiceEndpoint(typeof(WcfChannel<WcfMessageEnvelope>), new NetNamedPipeBinding(), pipeName);
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

		~WcfChannelHost()
		{
			Dispose(false);
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
	}


	/// <summary>
	/// Receives messages from a named pipe via WCF and forwards it to the output
	/// channel. Message serialization is handled entirely by WCF and therefore
	/// requires that classes are appropriately decorated.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class WcfChannelHost<T> :
		IDisposable
	{
		private readonly WcfChannelService<T> _service;
		private readonly Uri _serviceUri;

		private bool _disposed;
		private ServiceHost _serviceHost;

		public WcfChannelHost(Channel<T> output, Uri serviceUri, string pipeName)
		{
			_serviceUri = serviceUri;

			_service = new WcfChannelService<T>(output);

			_serviceHost = new ServiceHost(_service, _serviceUri);
			_serviceHost.AddServiceEndpoint(typeof (WcfChannel<T>), new NetNamedPipeBinding(), pipeName);
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

		~WcfChannelHost()
		{
			Dispose(false);
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
	}
}