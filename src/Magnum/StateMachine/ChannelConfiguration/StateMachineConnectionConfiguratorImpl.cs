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
namespace Magnum.StateMachine.ChannelConfiguration
{
	using System;
	using Magnum.Channels;
	using Magnum.Channels.Configuration.Internal;
	using Magnum.Extensions;
	using Magnum.Fibers;
	using Magnum.Fibers.Configuration;
	using Magnum.Logging;
	using Magnum.Reflection;


	public class StateMachineConnectionConfiguratorImpl<T> :
		StateMachineConnectionConfigurator<T>,
		ChannelConfigurator
		where T : StateMachine<T>
	{
		ChannelConfigurator _configurator;

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException("No state machine binder was specified for: " + typeof(T).ToShortTypeName());

			_configurator.ValidateConfiguration();
		}

		public void Configure(CreateChannelConnection connection, UntypedChannel channel)
		{
			_configurator.Configure(connection, channel);
		}

		public StateMachineConnectionConfigurator<T, TKey, TBinding> BindUsing<TBinding, TKey>()
			where TBinding : StateMachineBinding<T, TKey>
		{
			var configurator = new StateMachineConnectionConfiguratorImpl<T, TKey, TBinding>();

			_configurator = configurator;

			return configurator;
		}
	}


	public class StateMachineConnectionConfiguratorImpl<T, TKey, TBinding> :
		FiberProviderConfigurator<StateMachineConnectionConfigurator<T, TKey, TBinding>, TKey>,
		StateMachineConnectionConfigurator<T, TKey, TBinding>,
		ChannelConfigurator
		where T : StateMachine<T>
		where TBinding : StateMachineBinding<T, TKey>
	{
		static readonly ILogger _log = Logger.GetLogger<StateMachineConnectionConfiguratorImpl<T, TKey, TBinding>>();

		TBinding _binding;
		ChannelProviderFactory<T, TKey> _channelProviderFactory;
		FiberProvider<TKey> _fiberProvider;
		Func<TKey, T> _instanceFactory;

		public StateMachineConnectionConfiguratorImpl()
		{
			ExecuteOnThreadPoolFiber();
		}

		public void ValidateConfiguration()
		{
			if (_instanceFactory == null)
				throw new ChannelConfigurationException("No instance provider for state machine: " + typeof(T).ToShortTypeName());

			_binding = FastActivator<TBinding>.Create();

			_log.Debug(x => x.Write("Validating StateMachineBinding for {0}", typeof(T).ToShortTypeName()));

			_binding.Validate(_instanceFactory(default(TKey)));
		}

		public void Configure(CreateChannelConnection connection, UntypedChannel channel)
		{
			_fiberProvider = base.GetConfiguredProvider();

			_log.Debug(x => x.Write("Configuring State Machine Binder for {0}", typeof(T).ToShortTypeName()));

			_binding.ForEachEvent((@event, binder, result) =>
				{
					_log.Debug(x => x.Write("Binding Event: " + @event.Name));

					this.FastInvoke(new[] {result.EventType}, "ConfigureChannel", connection, channel, @event, binder, result);
				});
		}

		public FiberProvider<TKey> GetConfiguredProvider()
		{
			return _fiberProvider;
		}

		public void SetNewInstanceFactory(Func<TKey, T> instanceFactory)
		{
			_instanceFactory = instanceFactory;
		}

		public Func<TKey, T> GetConfiguredInstanceFactory()
		{
			return _instanceFactory;
		}

		public void SetProviderFactory(ChannelProviderFactory<T, TKey> factory)
		{
			_channelProviderFactory = factory;
		}

		public void ConfigureChannel<TChannel>(CreateChannelConnection connection, UntypedChannel channel,
		                                       DataEvent<T, TChannel> @event, EventBinder<T, TKey, TChannel> binder,
		                                       StateMachineEventInspectorResult<T> result)
		{
			_log.Debug(x => x.Write("Configuring channel for event {0}, message type {1}", @event.Name,
			                        typeof(TChannel).ToShortTypeName()));


			Func<TChannel, TKey> accessor = binder.GetBinder<TChannel>();
			KeyAccessor<TChannel, TKey> keyAccessor = m => accessor(m);

			ChannelAccessor<T, TChannel> channelAccessor =
				instance => new DelegateChannel<TChannel>(msg => instance.RaiseEvent(@event, msg));

			ChannelProvider<TChannel> channelProvider = _channelProviderFactory.GetChannelProvider(channelAccessor, keyAccessor);

			var keyedProvider = new KeyedChannelProvider<TChannel, TKey>(channelProvider, keyAccessor);

			var newChannel = new InstanceChannel<TChannel>(keyedProvider);

			new ConnectChannelVisitor<TChannel>(newChannel).ConnectTo(channel);

			connection.AddChannel(newChannel);
		}
	}
}