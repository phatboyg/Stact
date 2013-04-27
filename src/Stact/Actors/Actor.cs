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
namespace Stact
{
    using System;
    using Configuration;
    using Configuration.Internal;
    using Internals.Caching;
    using Internals.Extensions;


    public static class Actor
    {
        static Cache<Type, ActorFactory> _factoryCache;

        static Cache<Type, ActorFactory> FactoryCache
        {
            get { return _factoryCache ?? (_factoryCache = CreateFactoryCache()); }
        }

        static GenericTypeCache<ActorFactory> CreateFactoryCache()
        {
            return new GenericTypeCache<ActorFactory>(typeof(ActorFactory<>));
        }

        public static ActorRef New<TState>(TState state)
        {
            ActorFactory factory = FactoryCache.Get(typeof(TState), _ => CreateFactory<TState>());

            ActorRef actor = factory.New(state);

            return actor;
        }

        public static ActorRef New<TState>(TState state, Action<Actor<TState>> initialCallback)
        {
            ActorFactory factory = FactoryCache.Get(typeof(TState), _ => CreateFactory<TState>());

            var actorFactory = factory as ActorFactory<TState>;
            if (actorFactory == null)
            {
                throw new ArgumentException("Factory should be convertible to " + typeof(TState).GetTypeName()
                                            + " but was not");
            }

            Actor<TState> actor = actorFactory.New(state);

            actor.Internals.Fiber.Execute(() => initialCallback(actor));

            return actor.Self;
        }

        public static ActorRef New<TState>(Action<Actor<TState>> initialCallback)
            where TState : new()
        {
            var state = new TState();

            return New(state, initialCallback);
        }

        public static ActorFactory<TState> Get<TState>()
        {
            if (FactoryCache.Has(typeof(TState)))
            {
                var actorFactory = FactoryCache[typeof(TState)] as ActorFactory<TState>;
                if (actorFactory == null)
                {
                    throw new ArgumentException("Factory should be convertible to " + typeof(TState).GetTypeName()
                                                + " but was not");
                }
                return actorFactory;
            }

            throw new StactException("An actor factory for the requested type was not configured.");
        }

        public static ActorFactory Get(Type stateType)
        {
            if (FactoryCache.Has(stateType))
                return FactoryCache[stateType];

            throw new StactException("An actor factory for the requested type was not configured.");
        }


        /// <summary>
        /// Configures an actor factory for the specified state type, throwing an error if an actor factory has
        /// already been configured for the same state type.
        /// </summary>
        /// <typeparam name="TState">The type of state for the actor</typeparam>
        /// <param name="configureCallback">The configuration callback</param>
        /// <returns>The configured actor factory</returns>
        public static ActorFactory<TState> Configure<TState>(Action<ActorFactoryConfigurator<TState>> configureCallback)
        {
            ActorFactory<TState> actorFactory = CreateFactory(configureCallback);

            FactoryCache.Add(typeof(TState), actorFactory);

            return actorFactory;
        }

        /// <summary>
        /// Initialize an actor factory for the specified state type, replacing a previous configuration if one
        /// exists.
        /// </summary>
        /// <typeparam name="TState">The type of state for the actor</typeparam>
        /// <param name="configureCallback">The configuration callback for the actor factory</param>
        /// <returns>The configured actor factory</returns>
        public static ActorFactory<TState> Initialize<TState>(Action<ActorFactoryConfigurator<TState>> configureCallback)
        {
            ActorFactory<TState> actorFactory = CreateFactory(configureCallback);

            FactoryCache[typeof(TState)] = actorFactory;

            return actorFactory;
        }

        static ActorFactory<TState> CreateFactory<TState>()
        {
            return new ActorFactoryConfiguratorImpl<TState>()
                .Configure();
        }

        static ActorFactory<TState> CreateFactory<TState>(Action<ActorFactoryConfigurator<TState>> configurator)
        {
            var factoryConfiguratorImpl = new ActorFactoryConfiguratorImpl<TState>();

            configurator(factoryConfiguratorImpl);

            ActorFactory<TState> actorFactory = factoryConfiguratorImpl.Configure();
            return actorFactory;
        }
    }


    /// <summary>
    /// An Actor encapsulates the behavior and state of an actor in memory. This interface is not
    /// meant to be implemented by the library user.
    /// </summary>
    /// <typeparam name="TState">The type of state maintained for the actor</typeparam>
    public interface Actor<out TState> :
        UntypedActor
    {
        TState State { get; }

        /// <summary>
        /// The internals are meant to be used for specific purposes and should not generally be used.
        /// </summary>
        ActorInternals Internals { get; }

        /// <summary>
        /// Apply changes the behavior of an actor to the specified behavior, disposing of the previously
        /// active behavior.
        /// </summary>
        /// <typeparam name="TBehavior">The type of behavior to apply to the actor</typeparam>
        void ChangeBehavior<TBehavior>()
            where TBehavior : class, Behavior<TState>;
    }
}