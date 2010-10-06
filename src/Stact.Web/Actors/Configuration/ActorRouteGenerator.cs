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
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Routing;
	using Channels;
	using Magnum.Extensions;

	public static class ActorRouteGenerator
	{
		public static RouteValueDictionary GetRouteValuesForActor<TActor, TInput>(Expression<Func<TActor, Channel<TInput>>> expression)
		{
			Magnum.Guard.AgainstNull(expression, "expression");

			PropertyInfo property = expression.GetMemberPropertyInfo();

			return GetRouteValuesForActor<TActor>(property);
		}

		public static RouteValueDictionary GetRouteValuesForActor<TActor>(PropertyInfo property)
		{
			string actorName = GetActorName(typeof (TActor));

			string channelName = GetChannelName(property);

			var rvd = new RouteValueDictionary();
			rvd.Add("Actor", actorName);
			rvd.Add("Channel", channelName);

			return rvd;
		}

		private static string GetActorName(Type actorType)
		{
			const string suffix = "Actor";

			string name = actorType.Name;

			bool removeSuffix = name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);

			name = removeSuffix ? name.Substring(0, name.Length - suffix.Length) : name;

			if (name.Length == 0)
				throw new ArgumentException("Could not create a route to the actor: " + actorType.Name);

			return name;
		}

		private static string GetChannelName(PropertyInfo property)
		{
			const string suffix = "Channel";

			string name = property.Name;

			bool removeSuffix = name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);

			name = removeSuffix ? name.Substring(0, name.Length - suffix.Length) : name;

			if (name.Length == 0)
				throw new ArgumentException("Could not create a route to the channel: " + property.Name);

			return name;
		}
	}
}