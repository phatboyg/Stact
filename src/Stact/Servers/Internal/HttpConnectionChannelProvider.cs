// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.Servers.Internal
{
	using System.Collections.Generic;
	using System.Linq;
	using Channels;


	/// <summary>
	/// Returns a channel to process the request, mapping the request Uri to the
	/// channel implementation that support the specified URI.
	/// </summary>
	public class HttpConnectionChannelProvider :
		ChannelProvider<ConnectionContext>
	{
		ChannelProvider<ConnectionContext>[] _providers;
		ConnectionHandler _badRequestHandler;

		public HttpConnectionChannelProvider(IEnumerable<ChannelProvider<ConnectionContext>> providers)
		{
			_providers = providers.ToArray();
			_badRequestHandler = new BadRequestConnectionHandler();
		}

		public Channel<ConnectionContext> GetChannel(ConnectionContext context)
		{
			foreach (var provider in _providers)
			{
				Channel<ConnectionContext> channel = provider.GetChannel(context);
				if (null != channel)
					return channel;
			}

			return _badRequestHandler.GetChannel(context);
		}
	}
}