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
	using System.Web.Routing;
	using Configuration;

	public static class ConfigurationExtensions
	{
		/// <summary>
		/// Creates a configuration context to allow actors to be added to the routing table
		/// </summary>
		/// <param name="routes">The route collection to update</param>
		/// <param name="configure">The configuration action</param>
		public static void ConfigureActors(this RouteCollection routes, Action<RouteConfigurator> configure)
		{
			var configurator = new WebRoutingRouteConfiguration(routes);

			configure(configurator);

			configurator.Apply();
		}
	}
}