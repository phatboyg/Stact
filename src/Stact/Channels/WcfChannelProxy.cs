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
	using Magnum.Extensions;
	using Magnum.Serialization;


	public class WcfChannelProxy :
		UntypedChannel
	{
		readonly Fiber _fiber;
		readonly WcfChannel<WcfMessageEnvelope> _proxy;

		public WcfChannelProxy(Fiber fiber, Uri serviceUri, string pipeName)
		{
			_fiber = fiber;
			Serializer = new FastTextSerializer();
			ServiceUri = serviceUri;
			PipeName = pipeName;

			var channelFactory = new ConfigurationFreeChannelFactory<WcfChannel<WcfMessageEnvelope>>(serviceUri, pipeName);

			_proxy = channelFactory.CreateChannel();
		}

		public Serializer Serializer { get; private set; }

		public Uri ServiceUri { get; private set; }

		public string PipeName { get; private set; }

		public void Send<T>(T message)
		{
			_fiber.Add(() =>
				{
					var envelope = new WcfMessageEnvelope
						{
							MessageType = typeof(T).AssemblyQualifiedName,
							Body = Serializer.Serialize(message),
						};

					_proxy.Send(envelope);
				});
		}
	}


	public class WcfChannelProxy<T> :
		Channel<T>
	{
		readonly Fiber _fiber;
		readonly WcfChannel<T> _proxy;

		public WcfChannelProxy(Fiber fiber, Uri serviceUri, string pipeName)
		{
			_fiber = fiber;
			ServiceUri = serviceUri;
			PipeName = pipeName;

			var channelFactory = new ConfigurationFreeChannelFactory<WcfChannel<T>>(serviceUri, pipeName);

			_proxy = channelFactory.CreateChannel();
		}

		public Uri ServiceUri { get; private set; }

		public string PipeName { get; private set; }

		public void Send(T message)
		{
			_fiber.Add(() => _proxy.Send(message));
		}
	}
}