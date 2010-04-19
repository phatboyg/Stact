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
	using System.Web.Routing;
	using Actors.Echo;
	using Magnum.Actions;
	using Magnum.Logging;
	using Magnum.Web;
	using Magnum.Web.Actors;
	using Magnum.Web.Binding;

	public class Bootstrapper
	{
		public void Bootstrap(RouteCollection routeCollection)
		{
			TraceLogProvider.Configure(LogLevel.Debug);

			ModelBinder modelBinder = new FastModelBinder();

			RouteBuilder routeBuilder = new ActorRouteBuilder("actors/", modelBinder, routeCollection.Add);

			var actor = new EchoActor(new ThreadPoolActionQueue());

			routeBuilder.BuildRoute(() => actor, x => x.EchoChannel);

		}
	}
}