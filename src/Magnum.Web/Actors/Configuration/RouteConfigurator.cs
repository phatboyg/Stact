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
	using Magnum.Actors;


	/// <summary>
	/// Configures the actors into the routing collection
	/// </summary>
	public interface RouteConfigurator
	{
		/// <summary>
		/// Specifies the base path to prefix the URL for the actor (eg; "Actors" to get /Actors/{Actor}/{Channel})
		/// </summary>
		/// <param name="basePath"></param>
		void UseBasePath(string basePath);

		/// <summary>
		/// Add an actor to be added to the route collection
		/// </summary>
		/// <typeparam name="TActor"></typeparam>
		ActorConfigurator<TActor> Add<TActor>()
			where TActor : class, Actor;
	}
}