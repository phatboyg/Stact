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
namespace Magnum.Web.Actors
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Actions;
	using Channels;
	using Extensions;
	using Logging;
	using Reflection;

	public class DefaultActorRouteConfigurator<TActor> :
		ApplyActorRouteConfigurator
		where TActor : class
	{
		private static readonly ILogger _log = Logger.GetLogger<DefaultActorRouteConfigurator<TActor>>();
		private ActorInstanceProvider<TActor> _actorInstanceProvider;

		private Action<AddRouteConfigurator> _configure;
		private ActionQueueProvider _queueProvider;

		public DefaultActorRouteConfigurator()
		{
			_queueProvider = ThreadPoolQueueProvider;
			_actorInstanceProvider = new TransientActorInstanceProvider<TActor>(_queueProvider);
			_configure = DefaultConfigureAction;
		}

		public ActorRouteConfigurator All()
		{
			var actions = new List<Action<AddRouteConfigurator>>();

			Type actorType = typeof (TActor);

			actorType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.PropertyType.Implements<Channel>())
				.Each(property =>
					{
						Type inputType = property.PropertyType.GetGenericTypeDeclarations(typeof (Channel<>)).Single();

						_log.Debug(x => x.Write("Configuring Channel<{0}> for {1}", inputType.Name, typeof (TActor).Name));

						actions.Add(x =>
							{
								var args = new object[] {x, property};

								this.FastInvoke(new[] {inputType}, "AddRoute", args);
							});
					});

			if (actions.Count == 0)
				throw new ArgumentException("No channels were found for actor: " + typeof (TActor).Name);

			_configure = configurator => actions.Each(r => r(configurator));

			return this;
		}

		public void Apply(AddRouteConfigurator configurator)
		{
			_configure(configurator);
		}

		private void AddRoute<TInput>(AddRouteConfigurator configurator, PropertyInfo property)
		{
			var channelProvider = new ActorChannelProvider<TActor, TInput>(_actorInstanceProvider, property);

			configurator.AddRoute<TActor, TInput>(channelProvider, property);
		}

		private static void DefaultConfigureAction(RouteConfigurator obj)
		{
			throw new InvalidOperationException("No channels have been specified for the actor: " + typeof (TActor).Name);
		}

		private static ActionQueue ThreadPoolQueueProvider()
		{
			return new ThreadPoolActionQueue();
		}
	}
}