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
namespace Stact.Routing.Configuration
{
	using System;
	using System.Collections.Generic;
	using Routing.Internal;
	using Stact.Configuration;
	using Stact.Configuration.Internal;
	using Stact.Internal;


	public class WhenConfiguratorImpl<T> :
		WhenConfigurator<T>
	{
		ChannelConfigurator<T> _configurator;

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException(typeof(T), "No channel configurator was setup");

			_configurator.ValidateConfiguration();
		}

		public RoutingConfiguration Configure(RoutingEngine engine)
		{
			new AlphaNodeSelector<T>().Select(engine, alphaNode =>
				{
					ConsumerNode<T> consumerNode = null;

					var locator = new JoinNodeLocator<T>(joinNode =>
						{
							var configurator = new NodeChannelConfiguratorConnectionImpl<T>(joinNode);
							_configurator.Configure(configurator);
						});
				});

			throw new NotImplementedException();
		}

		public void SetChannelConfigurator(ChannelConfigurator<T> configurator)
		{
			_configurator = configurator;
		}
	}

	public class NodeChannelConfiguratorConnectionImpl<TChannel> :
		ChannelConfiguratorConnection<TChannel>
	{
		readonly JoinNode<TChannel> _joinNode;
		readonly IList<IDisposable> _disposables;

		public NodeChannelConfiguratorConnectionImpl(JoinNode<TChannel> joinNode)
		{
			_joinNode = joinNode;

			_disposables = new List<IDisposable>();
		}

		public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
		{
			Channel<TChannel> channel = channelFactory(fiber);

			var channelNode = new ChannelNode<TChannel>(channel, _disposables);

			_joinNode.AddActivation(channelNode);
		}

		public void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory)
		{
			throw new InvalidOperationException("Not sure this is allowed yet");
		}

		public void AddDisposable(IDisposable disposable)
		{
			//_connection.AddDisposable(disposable);
		}
	}
}