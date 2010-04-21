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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Web.Routing;
	using Actions;
	using Binding;
	using Channels;
	using Extensions;

	public class RoutingRouteConfigurator :
		AddRouteConfigurator
	{
		private readonly RouteCollection _routeCollection;
		private List<ApplyActorRouteConfigurator> _actors = new List<ApplyActorRouteConfigurator>();
		private ModelBinder _modelBinder = new FastModelBinder();

		public RoutingRouteConfigurator(RouteCollection routeCollection)
		{
			_routeCollection = routeCollection;
			BasePath = "";
		}

		protected string BasePath { get; private set; }

		public void UseBasePath(string basePath)
		{
			BasePath = basePath ?? "";
		}

		public ActorRouteConfigurator Add<TActor>()
			where TActor : class
		{
			var actorRouteConfigurator = new DefaultActorRouteConfigurator<TActor>();
			_actors.Add(actorRouteConfigurator);

			return actorRouteConfigurator;
		}

		public void AddRoute<TActor, TInput>(ChannelProvider<TInput> provider, PropertyInfo property)
		{
			var routeHandler = new ActorRouteHandler<TInput>(() => new ThreadPoolActionQueue(), _modelBinder, provider);

			RouteValueDictionary routeValueDictionary = ActorRouteGenerator.GetRouteValuesForActor<TActor>(property);

			string url = BasePath.TrimEnd('/') + "/{Actor}/{Channel}";

			var route = new Route(url, new RouteValueDictionary(), routeValueDictionary, routeHandler);

			_routeCollection.Add(route);
		}

		public void Apply()
		{
			_actors.Each(x => x.Apply(this));
		}

		private string GetUrl(RouteValueDictionary routeValueDictionary)
		{
			var actorName = routeValueDictionary.StrongGet<string>("Actor");
			var channelName = routeValueDictionary.StrongGet<string>("Channel");

			Guard.AgainstEmpty(actorName, "Actor");
			Guard.AgainstEmpty(channelName, "Channel");

			return string.Format("{0}/{1}/{2}", BasePath.TrimEnd('/'), actorName.Trim('/'), channelName.Trim('/'));
		}
	}
}