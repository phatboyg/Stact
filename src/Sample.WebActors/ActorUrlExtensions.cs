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
namespace Sample.WebActors
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Web.Mvc;
	using System.Web.Routing;
	using Stact;
	using Stact.Channels;
	using Magnum.Extensions;
	using Stact.Web.Actors.Configuration;

	public static class ActorUrlExtensions
	{
		public static string Actor<TActor>(this UrlHelper url, Expression<Func<TActor, object>> expression)
		{
			Magnum.Guard.AgainstNull(expression, "expression");

			PropertyInfo property = expression.GetMemberPropertyInfo();

			Magnum.Guard.IsTrue(x => x.Implements(typeof (Channel<>)), property.PropertyType, "Property must be a channel");

			RouteValueDictionary rvd = ActorRouteGenerator.GetRouteValuesForActor<TActor>(property);

			VirtualPathData vpd = url.RouteCollection.GetVirtualPath(url.RequestContext, rvd);

			return (vpd == null) ? null : vpd.VirtualPath;
		}
	}
}