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
namespace Stact
{
	using System;
	using System.ServiceModel;
	using Internal;
	using Magnum.Serialization;


	public class WcfChannelProxy :
		UntypedChannel,
		IDisposable
	{
		readonly Fiber _fiber;
		ConfigurationFreeChannelFactory<WcfChannel<WcfMessageEnvelope>> _channelFactory;

		public WcfChannelProxy(Fiber fiber, Uri serviceUri, string pipeName)
		{
			_fiber = fiber;
			Serializer = new FastTextSerializer();
			ServiceUri = serviceUri;
			PipeName = pipeName;

			_channelFactory = new ConfigurationFreeChannelFactory<WcfChannel<WcfMessageEnvelope>>(serviceUri, pipeName);
			try
			{
				_channelFactory.Open();
			}
			catch (Exception ex)
			{
				_channelFactory.Abort();
				_channelFactory = null;
				throw;
			}
		}

		public Serializer Serializer { get; private set; }

		public Uri ServiceUri { get; private set; }

		public string PipeName { get; private set; }

		public void Dispose()
		{
			if (_channelFactory != null)
			{
				try
				{
					_channelFactory.Close();
				}
				finally
				{
					_channelFactory = null;
				}
			}
		}

		public void Send<T>(T message)
		{
			_fiber.Add(() =>
			{
				try
				{
					var envelope = new WcfMessageEnvelope
					{
						MessageType = typeof(T).AssemblyQualifiedName,
						Body = Serializer.Serialize(message),
					};

					WcfChannel<WcfMessageEnvelope> proxy = _channelFactory.CreateChannel();
					proxy.Send(envelope);

					var channel = proxy as IClientChannel;
					if (channel != null)
						channel.Close();

					var disposable = proxy as IDisposable;
					if (disposable != null)
						disposable.Dispose();
				}
				catch
				{
					// i hate doing this, but we don't want to take down the entire appdomain
				}
			});
		}
	}


	public class WcfChannelProxy<T> :
		Channel<T>
	{
		readonly Fiber _fiber;
		ConfigurationFreeChannelFactory<WcfChannel<T>> _channelFactory;

		public WcfChannelProxy(Fiber fiber, Uri serviceUri, string pipeName)
		{
			_fiber = fiber;
			ServiceUri = serviceUri;
			PipeName = pipeName;

			_channelFactory = new ConfigurationFreeChannelFactory<WcfChannel<T>>(serviceUri, pipeName);
			try
			{
				_channelFactory.Open();
			}
			catch (Exception ex)
			{
				_channelFactory.Abort();
				_channelFactory = null;
				throw;
			}
		}

		public Uri ServiceUri { get; private set; }

		public string PipeName { get; private set; }

		public void Send(T message)
		{
			_fiber.Add(() =>
			{
				try
				{
					WcfChannel<T> proxy = _channelFactory.CreateChannel();
					proxy.Send(message);

					var channel = proxy as IClientChannel;
					if (channel != null)
						channel.Close();

					var disposable = proxy as IDisposable;
					if (disposable != null)
						disposable.Dispose();
				}
				catch
				{
					// i hate doing this, but we don't want to take down the entire appdomain
				}
			});
		}
	}
}