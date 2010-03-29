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
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Routing;
	using Channels;
	using InterfaceExtensions;

	public class ActorRouteBuilder :
		RouteBuilder
	{
		public void BuildRoute(Type actorType, Action<Route> routeAction)
		{
			string prefix = "actor/";

			string actorName = actorType.Name;

			string actor = actorName.EndsWith("Actor") ? actorName.Substring(0, actorName.Length - 5) : actorName;

			actorType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.Implements<Channel>())
				.Each(property =>
					{
						string channel = property.Name.EndsWith("Channel") ? property.Name.Substring(0, property.Name.Length - 7) : property.Name;

						string url = string.Format("{0}{1}/{2}", prefix, actor, channel);

						var route = new Route(url, new ActorRouteHandler());

						routeAction(route);
					});
		}

		public void BuildRoute<TActor, TChannel>(Expression<Func<TActor, Channel<TChannel>>> channelExpression, Action<Route> routeAction)
		{
			//throw new NotImplementedException();
		}
	}
}