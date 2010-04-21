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
	using Actors.Query;
	using Magnum.Actions;
	using Magnum.Channels;
	using Magnum.Logging;
	using Magnum.Web;
	using Magnum.Web.Actors;

	public class Bootstrapper
	{
		public void Bootstrap(RouteCollection routeCollection)
		{
			TraceLogProvider.Configure(LogLevel.Debug);

			routeCollection.ConfigureActors(x =>
				{
					x.UseBasePath("actors");

					x.Add<EchoActor>().All();
					x.Add<QueryActor>().All();
				});
		}

		private void RegisterActor(RouteBuilder routeBuilder)
		{
			var inputProvider = new DelegateChannelProvider<EchoInputModel>(x => new EchoActor(new ThreadPoolActionQueue()).EchoChannel);
			var provider = new ThreadStaticChannelProvider<EchoInputModel>(inputProvider);

			routeBuilder.BuildRoute<EchoActor, EchoInputModel>(x => x.EchoChannel, provider);
		}

		private void RegisterActor2(RouteBuilder routeBuilder)
		{
			var inputProvider = new DelegateChannelProvider<QueryInputModel>(x => new QueryActor(new ThreadPoolActionQueue()).GetCityChannel);
			var provider = new ThreadStaticChannelProvider<QueryInputModel>(inputProvider);

			routeBuilder.BuildRoute<QueryActor, QueryInputModel>(x => x.GetCityChannel, provider);
		}
	}
}