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
namespace Magnum.Web.Actors.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Web.Routing;
	using Binding;
	using Channels;
	using Extensions;
	using Fibers;
	using Internal;
	using Magnum.Actors;


	public class WebRoutingRouteConfiguration :
		RouteConfiguration
	{
		private readonly List<Action<RouteConfiguration>> _actors = new List<Action<RouteConfiguration>>();
		private readonly ModelBinder _modelBinder = new FastModelBinder();
		private readonly RouteCollection _routeCollection;

		public WebRoutingRouteConfiguration(RouteCollection routeCollection)
		{
			_routeCollection = routeCollection;
			BasePath = "";
		}

		protected string BasePath { get; private set; }

		public void UseBasePath(string basePath)
		{
			BasePath = basePath ?? "";
		}

		public void AddRoute<TActor, TInput>(ChannelProvider<TInput> provider, PropertyInfo property)
		{
			var routeHandler = new ActorRouteHandler<TInput>(() => new ThreadPoolFiber(), _modelBinder, provider);

			RouteValueDictionary routeValueDictionary = ActorRouteGenerator.GetRouteValuesForActor<TActor>(property);

			string url = BasePath.TrimEnd('/') + "/{Actor}/{Channel}";

			var route = new Route(url, new RouteValueDictionary(), routeValueDictionary, routeHandler);

			_routeCollection.Add(route);
		}

		public ActorConfigurator<TActor> Add<TActor>()
			where TActor : class, Actor
		{
			var actorRouteConfigurator = new StandardActorConfiguration<TActor>();
			_actors.Add(actorRouteConfigurator.Apply);

			return actorRouteConfigurator;
		}

		public void Apply()
		{
			_actors.Each(x => x(this));
		}
	}
}