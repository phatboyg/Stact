namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Web.Routing;
	using Channels;
	using CommandQueues;
	using Simple;
	using StructureMap;
	using StructureMap.Attributes;
	using WebPage;

	public static class Bootstrapper
	{
		public static void WithEach<T>(Action<T> action)
		{
			foreach (var configuration in ObjectFactory.Model.PluginTypes)
			{
				if (!configuration.Implements<T>()) continue;

				var instance = (T) ObjectFactory.GetInstance(configuration.PluginType);

				action(instance);
			}
		}

		public static bool Implements<T>(this PluginTypeConfiguration configuration)
		{
			return configuration.PluginType.Implements<T>();
		}

		public static bool Implements<T>(this Type type)
		{
			return typeof (T).IsAssignableFrom(type);
		}

		public static void Start()
		{
			ObjectFactory.Initialize(x =>
				{
					x.ForRequestedType<Channel<SimpleRequest>>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<SynchronousChannel<SimpleRequest>>();

					x.ForRequestedType<Channel<SimpleResponse>>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<SynchronousChannel<SimpleResponse>>();

					x.ForRequestedType<Channel<GetWebPage>>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<SynchronousChannel<GetWebPage>>();

					x.ForRequestedType<Channel<WebPageContent>>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<SynchronousChannel<WebPageContent>>();

					x.ForRequestedType<CommandQueue>()
						.TheDefault.Is.OfConcreteType<ThreadPoolCommandQueue>();

					x.ForRequestedType<ChannelFactory>()
						.TheDefault.Is.OfConcreteType<StructureMapChannelFactory>();

					x.ForRequestedType<WebPageRetrieval>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<WebPageRetrievalService>();

					x.ForRequestedType<IRequestService>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<RequestService>();

					x.ForRequestedType<SimpleRequestActor>()
						.TheDefault.Is.OfConcreteType<SimpleRequestActor>();

					x.ForRequestedType<GetWebPageActor>()
						.TheDefault.Is.OfConcreteType<GetWebPageActor>();
				});

			WithEach<IStartable>(x => x.Start());

			RegisterRoutes();
		}

		private static void RegisterRoutes()
		{
			WithEach<AsyncHttpActor>(x =>
				{
					Type handlerType = x.GetType();

					string routeUrl = handlerType.Name;
					if (routeUrl.IndexOf("Actor") == routeUrl.Length - 5)
						routeUrl = routeUrl.Substring(0, routeUrl.Length - 5);

					var route = new Route(routeUrl, new ActorRouteHandler(handlerType));

					RouteTable.Routes.Add(route);
				});

//			Route simpleRequestRoute = new Route("{Actor}", new ActorRouteHandler());
//			RouteTable.Routes.Add(simpleRequestRoute);
		}
	}
}