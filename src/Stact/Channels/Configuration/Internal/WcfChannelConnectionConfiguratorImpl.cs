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
	using Fibers;
	using Fibers.Configuration;


	/// <summary>
	/// Exposes the configuration options for a WcfProxy
	/// </summary>
	public class WcfChannelConnectionConfiguratorImpl :
		FiberConfiguratorImpl<WcfChannelConnectionConfigurator>,
		WcfChannelConnectionConfigurator,
		ChannelConfigurator
	{
		readonly Uri _endpointUri;
		readonly string _pipeName;

		public WcfChannelConnectionConfiguratorImpl(Uri endpointUri, string pipeName)
		{
			Magnum.Guard.AgainstNull(endpointUri);
			Magnum.Guard.AgainstNull(pipeName);

			_endpointUri = endpointUri;
			_pipeName = pipeName;
		}

		public void ValidateConfiguration()
		{
		}

		public void Configure(ChannelConfiguratorConnection connection)
		{
			Fiber fiber = GetConfiguredFiber(connection);

			connection.AddChannel(fiber, x => new WcfChannelProxy(x, _endpointUri, _pipeName));
		}
	}
}