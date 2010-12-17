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
	using System.Collections.Generic;
	using Builders;
	using Magnum.Extensions;


	public class UntypedConnectionConfigurator :
		ConnectionConfigurator
	{
		readonly UntypedChannel _channel;
		readonly IList<ConnectionBuilderConfigurator> _configurators;

		public UntypedConnectionConfigurator(UntypedChannel channel)
		{
			_channel = channel;
			_configurators = new List<ConnectionBuilderConfigurator>();
		}

//		public void AddChannel<TChannel>(Channel<TChannel> channel)
//		{
//			var configurator = new TypedChannelConfigurator<TChannel>(channel);
//
//			_configurators.Add(configurator);
//		}
//
//		public void AddUntypedChannel(UntypedChannel channel)
//		{
//			var configurator = new UntypedChannelConfigurator(channel);
//
//			_configurators.Add(configurator);
//		}

		public void AddConfigurator(ConnectionBuilderConfigurator configurator)
		{
			_configurators.Add(configurator);
		}

		public ChannelConnection CreateConnection()
		{
			_configurators.Each(x => x.ValidateConfiguration());

			var connection = new UntypedChannelConnectionBuilder(_channel);

			_configurators.Each(configurator => configurator.Configure(connection));

			return connection.ChannelConnection;
		}
	}
}