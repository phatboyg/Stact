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
	using Binding;
	using Channels;
	using InterfaceExtensions;
	using Reflection;

	public class ActorRouteBuilder :
		RouteBuilder
	{
		private readonly Action<Route> _addRoute;
		private readonly ModelBinder _modelBinder;
		private readonly string _prefix;

		public ActorRouteBuilder(string prefix, ModelBinder modelBinder, Action<Route> addRoute)
		{
			_addRoute = addRoute;
			_prefix = prefix;
			_modelBinder = modelBinder;
		}

		public void BuildRoute<TActor, TInput, TOutput>(Func<TActor> getActor, Expression<Func<TActor, Channel<TInput>>> channelAccessor)
			where TInput : HasOutputChannel<TOutput>
		{
			PropertyInfo property = channelAccessor.GetMemberPropertyInfo();

			Func<TActor, Channel<TInput>> compiled = channelAccessor.Compile();

			Func<Channel<TInput>> getChannel = () => compiled(getActor());

			ActorBinder binder = new BasicActorBinder<TInput, TOutput>(_modelBinder, getChannel);
			var routeHandler = new ActorRouteHandler(binder);

			string url = GetUrl(typeof (TActor).Name, property.Name);

			var route = new Route(url, routeHandler);

			_addRoute(route);
		}

		private string GetUrl(string actorName, string propertyName)
		{
			string actor = actorName.EndsWith("Actor") ? actorName.Substring(0, actorName.Length - 5) : actorName;
			string channel = propertyName.EndsWith("Channel") ? propertyName.Substring(0, propertyName.Length - 7) : propertyName;

			return string.Format("{0}{1}/{2}", _prefix, actor, channel);
		}

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

						Type inputType = property.PropertyType.GetDeclaredTypeForGeneric(typeof (Channel<>));

						Type outputType = inputType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
							.Where(x => x.Implements<Channel>())
							.Where(x => x.Name.StartsWith("Output"))
							.Select(x => x.PropertyType).Single();

						this.FastInvoke(new[] {actorType, inputType, outputType}, "BuildRoute", new object[] {property});

					});
		}
	}
}