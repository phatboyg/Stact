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
namespace Stact.Web.Actors.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Stact.Internal;


	public class StandardActorConfiguration<TActor> :
		ActorConfiguration<TActor>
		where TActor : class, Actor
	{
		readonly FiberFactory _fiberFactory;
		ActorFactory<TActor> _actorFactory;

		Action<RouteConfiguration> _configure;

		public StandardActorConfiguration()
		{
			_fiberFactory = ThreadPoolFiberProvider;
			_actorFactory = null;
			_configure = DefaultConfigureAction;
		}

		public ActorConfigurator All()
		{
			var actions = new List<Action<RouteConfiguration>>();

			Type actorType = typeof(TActor);

			actorType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.PropertyType.Implements<Channel>())
				.Each(property =>
					{
						Type inputType = property.PropertyType.GetGenericTypeDeclarations(typeof(Channel<>)).Single();


						actions.Add(configurator =>
							{
								var args = new object[] {configurator, property};

								this.FastInvoke(new[] {inputType}, "AddRoute", args);
							});
					});

			if (actions.Count == 0)
				throw new ArgumentException("No channels were found for actor: " + typeof(TActor).Name);

			_configure = configurator => actions.Each(r => r(configurator));

			return this;
		}

		public ActorConfigurator PerThread()
		{
			if (_actorFactory.GetType() == typeof(ThreadStaticActorFactory<TActor>))
				return this;

			_actorFactory = new ThreadStaticActorFactory<TActor>(_actorFactory);
			return this;
		}

		public ActorConfigurator<TActor> Channel<TChannel>(Expression<Func<TActor, TChannel>> expression)
		{
			PropertyInfo property = expression.GetMemberPropertyInfo();

			Type inputType = property.PropertyType.GetGenericTypeDeclarations(typeof(Channel<>)).Single();


			_configure = configurator =>
				{
					var args = new object[] {configurator, property};

					this.FastInvoke(new[] {inputType}, "AddRoute", args);

					// TODO need to make this keep track of each channel that needs to be added
				};

			return this;
		}

		public void Apply(RouteConfiguration configuration)
		{
			_configure(configuration);
		}

		void AddRoute<TInput>(RouteConfiguration configuration, PropertyInfo property)
		{
			var channelProvider = new ActorChannelProvider<TActor, TInput>(_actorFactory, property);

			configuration.AddRoute<TActor, TInput>(channelProvider, property);
		}

		static void DefaultConfigureAction(RouteConfigurator obj)
		{
			throw new InvalidOperationException("No channels have been specified for the actor: " + typeof(TActor).Name);
		}

		static Fiber ThreadPoolFiberProvider()
		{
			return new PoolFiber();
		}
	}
}