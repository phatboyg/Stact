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
namespace Stact.Configuration.Internal
{
	using System;
	using Builders;


	/// <summary>
	/// Exposes the configuration options for a WcfProxy
	/// </summary>
	public class WcfConnectionConfiguratorImpl :
		FiberFactoryConfiguratorImpl<WcfConnectionConfigurator>,
		WcfConnectionConfigurator,
		ConnectionBuilderConfigurator
	{
		readonly Uri _endpointUri;
		readonly string _pipeName;

		public WcfConnectionConfiguratorImpl(Uri endpointUri, string pipeName)
		{
			Guard.AgainstNull(endpointUri, "endpointUri");
			Guard.AgainstEmpty(pipeName, "pipeName");

			_endpointUri = endpointUri;
			_pipeName = pipeName;
		}

		public void ValidateConfiguration()
		{
		}

		public void Configure(ConnectionBuilder builder)
		{
			Fiber fiber = GetConfiguredFiber(builder);

			var proxy = new WcfChannelProxy(fiber, _endpointUri, _pipeName);

			builder.AddChannel(proxy);
			builder.AddDisposable(proxy);
		}
	}
}