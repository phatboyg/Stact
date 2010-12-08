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
	using Configuration;
	using Configuration.Internal;
	using Internal;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Magnum.StateMachine;

	public class StateMachineConnectionConfiguratorImpl<T> :
		StateMachineConnectionConfigurator<T>,
		ConnectionBuilderConfigurator
		where T : StateMachine<T>
	{
		ConnectionBuilderConfigurator _configurator;

		public void ValidateConfiguration()
		{
			if (_configurator == null)
				throw new ChannelConfigurationException("No state machine binder was specified for: " + typeof(T).ToShortTypeName());

			_configurator.ValidateConfiguration();
		}

		public void Configure(ConnectionBuilder builder)
		{
			_configurator.Configure(builder);
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
		ConnectionBuilderConfigurator
		where T : StateMachine<T>
		where TBinding : StateMachineBinding<T, TKey>
	{
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

			_binding.Validate(_instanceFactory(default(TKey)));
		}

		public void Configure(ConnectionBuilder builder)
		{
			_fiberProvider = GetConfiguredFiberProvider(builder);

			_binding.ForEachEvent((@event, binder, result) =>
				{
					this.FastInvoke(new[] {result.EventType}, "ConfigureChannel", builder, @event, binder,
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

		FiberProvider<TKey> GetConfiguredFiberProvider(ConnectionBuilder connection)
		{
			FiberProvider<TKey> configuredProvider = GetConfiguredFiberProvider();
			connection.AddDisposable(configuredProvider);

			return configuredProvider;
		}


		public void ConfigureChannel<TChannel>(ConnectionBuilder connection,
		                                       DataEvent<T, TChannel> @event, EventBinder<T, TKey, TChannel> binder,
		                                       StateMachineEventInspectorResult<T> result)
		{
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