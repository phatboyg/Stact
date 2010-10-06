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
namespace Stact.StateMachine.ChannelConfiguration
{
	using System;
	using Channels;
	using Magnum.Extensions;
	using Fibers;
	using Fibers.Configuration;
	using Magnum.Logging;
	using Stact.Channels;
	using Stact.Channels.Configuration.Internal;
	using Magnum.Reflection;
	using Magnum.StateMachine;


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

		public void Configure(ChannelConfiguratorConnection connection)
		{
			_configurator.Configure(connection);
		}

		public StateMachineConnectionConfigurator<T, TKey, TBinding> BindUsing<TBinding, TKey>()
			where TBinding : StateMachineBinding<T, TKey>
		{
			var configurator = new StateMachineConnectionConfiguratorImpl<T, TKey, TBinding>();

			_configurator = configurator;

			return configurator;
		}

		public StateMachineInstanceConnectionConfigurator<T> UsingInstance(T instance)
		{
			var instanceConfigurator = new StateMachineInstanceConnectionConfiguratorImpl<T>(instance);

			_configurator = instanceConfigurator;

			return instanceConfigurator;
		}
	}


	public class StateMachineConnectionConfiguratorImpl<T, TKey, TBinding> :
		FiberProviderConfiguratorImpl<StateMachineConnectionConfigurator<T, TKey, TBinding>, TKey>,
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
			HandleOnPoolFiber();
		}

		public void ValidateConfiguration()
		{
			if (_instanceFactory == null)
				throw new ChannelConfigurationException("No instance provider for state machine: " + typeof(T).ToShortTypeName());

			_binding = FastActivator<TBinding>.Create();

			_log.Debug(x => x.Write("Validating StateMachineBinding for {0}", typeof(T).ToShortTypeName()));

			_binding.Validate(_instanceFactory(default(TKey)));
		}

		public void Configure(ChannelConfiguratorConnection connection)
		{
			_fiberProvider = GetConfiguredFiberProvider(connection);

			_log.Debug(x => x.Write("Configuring State Machine Binder for {0}", typeof(T).ToShortTypeName()));

			_binding.ForEachEvent((@event, binder, result) =>
				{
					_log.Debug(x => x.Write("Binding Event: " + @event.Name));

					this.FastInvoke(new[] {result.EventType}, "ConfigureChannel", connection, @event, binder,
					                result);
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


		public void ConfigureChannel<TChannel>(ChannelConfiguratorConnection connection,
		                                       DataEvent<T, TChannel> @event, EventBinder<T, TKey, TChannel> binder,
		                                       StateMachineEventInspectorResult<T> result)
		{
			_log.Debug(x => x.Write("Configuring channel for event {0}, message type {1}", @event.Name,
			                        typeof(TChannel).ToShortTypeName()));

			Func<TChannel, TKey> accessor = binder.GetBinder<TChannel>();
			KeyAccessor<TChannel, TKey> keyAccessor = m => accessor(m);

			ChannelAccessor<T, TChannel> channelAccessor =
				instance => new StateMachineEventChannel<T, TChannel>(instance, @event);

			Func<TKey, T> missingInstanceProvider = GetConfiguredInstanceFactory();

			var delegateInstanceProvider = new DelegateInstanceProvider<T, TChannel>(msg =>
				{
					TKey key = keyAccessor(msg);

					T instance = missingInstanceProvider(key);

					return instance;
				});

			InstanceChannelPolicy<T, TChannel> policy = result.GetPolicy(delegateInstanceProvider);

			ChannelProvider<TChannel> channelProvider = _channelProviderFactory.GetChannelProvider(channelAccessor, keyAccessor,
			                                                                                       policy);

			var keyedProvider = new KeyedChannelProvider<TChannel, TKey>(channelProvider, keyAccessor);

			var fiber = new SynchronousFiber();

			connection.AddChannel(fiber, x => new InstanceChannel<TChannel>(x, keyedProvider));
		}
	}
}