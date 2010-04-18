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
	using Actions;
	using Binding;
	using Channels;
	using Extensions;
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

		public void BuildRoute<TActor, TInput>(Func<TActor> getActor, Expression<Func<TActor, Channel<TInput>>> channelAccessor)
		{
			PropertyInfo property = channelAccessor.GetMemberPropertyInfo();

			Func<TActor, Channel<TInput>> compiled = channelAccessor.Compile();

			Func<Channel<TInput>> getChannel = () => compiled(getActor());

			ActorBinder binder = new BasicActorBinder<TInput>(_modelBinder, getChannel);
			var routeHandler = new ActorRouteHandler(binder, () => new ThreadPoolActionQueue());

			string url = GetUrl(typeof (TActor).Name, property.Name);

			var route = new Route(url, routeHandler);

			_addRoute(route);
		}


		public void BuildRoute<TActor>(Func<TActor> getActor)
		{
			Type actorType = typeof (TActor);

			actorType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.Implements<Channel>())
				.Each(property =>
					{
						Type inputType = property.PropertyType.GetGenericTypeDeclarations(typeof(Channel<>)).Single();

						this.FastInvoke(new[] {actorType, inputType}, "BuildRoute", new object[] {getActor, property});
					});
		}

		public void BuildRoute<TActor, TInput>(Func<TActor> getActor, PropertyInfo property)
		{
			Type outputType = typeof (TInput).GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(x => x.Implements<Channel>())
				.Select(x => x.PropertyType).Single();

			throw new NotImplementedException();
		}

		private string GetUrl(string actorName, string propertyName)
		{
			string actor = actorName.EndsWith("Actor") ? actorName.Substring(0, actorName.Length - 5) : actorName;
			string channel = propertyName.EndsWith("Channel") ? propertyName.Substring(0, propertyName.Length - 7) : propertyName;

			return string.Format("{0}{1}/{2}", _prefix, actor, channel);
		}
	}
}