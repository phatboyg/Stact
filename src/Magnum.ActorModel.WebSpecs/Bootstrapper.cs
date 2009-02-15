namespace Magnum.ActorModel.WebSpecs
{
	using System;
	using System.Web;
	using System.Web.Compilation;
	using System.Web.Routing;
	using System.Web.UI;
	using Channels;
	using CommandQueues;
	using StructureMap;
	using StructureMap.Attributes;

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
						.TheDefault.Is.OfConcreteType<ChannelImpl<SimpleRequest>>();

					x.ForRequestedType<Channel<SimpleResponse>>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<ChannelImpl<SimpleResponse>>();

					x.ForRequestedType<CommandExecutor>()
						.TheDefault.Is.OfConcreteType<SynchronousCommandExecutor>();

					x.ForRequestedType<CommandQueue>()
						.TheDefault.Is.OfConcreteType<AsyncCommandQueue>()
						.WithCtorArg("limit").EqualTo(5000)
						.WithCtorArg("waitTime").EqualTo(1000);

					x.ForRequestedType<CommandContext>()
						.TheDefault.Is.OfConcreteType<ThreadCommandContext>();

					x.ForRequestedType<ThreadPoolCommandQueue>()
						.TheDefault.Is.OfConcreteType<ThreadPoolCommandQueue>();

					x.ForRequestedType<ChannelFactory>()
						.TheDefault.Is.OfConcreteType<StructureMapChannelFactory>();

					x.InstanceOf<RequestService>()
						.Is.OfConcreteType<RequestService>();

					x.ForRequestedType<ActorHttpAsyncHandler<SimpleRequestActor>>()
						.CacheBy(InstanceScope.Singleton)
						.TheDefault.Is.OfConcreteType<ActorHttpAsyncHandler<SimpleRequestActor>>();
				});

			WithEach<IStartable>(x => x.Start());

			RegisterRoutes();
		}

		private static void RegisterRoutes()
		{
			WithEach<IHttpAsyncHandler>(x =>
				{
					Type handlerType = x.GetType();
					if(!handlerType.IsGenericType ) return;

					Type genericType = handlerType.GetGenericTypeDefinition();
					if(genericType != typeof(ActorHttpAsyncHandler<>)) return;

					Type actorType = handlerType.GetGenericArguments()[0];

					string routeUrl = actorType.Name;
					if (routeUrl.IndexOf("Actor") == routeUrl.Length - 5)
						routeUrl = routeUrl.Substring(0, routeUrl.Length - 5);

					var route = new Route(routeUrl, new ActorRouteHandler(handlerType));

					RouteTable.Routes.Add(route);
				});

//			Route simpleRequestRoute = new Route("{Actor}", new ActorRouteHandler());
//			RouteTable.Routes.Add(simpleRequestRoute);
		}


	}

	public class ActorRouteHandler : IRouteHandler
	{
		private readonly Type _type;

		public ActorRouteHandler(Type type)
		{
			_type = type;
		}

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			object handler = ObjectFactory.GetInstance(_type);

			return (IHttpAsyncHandler) handler;
//
//
//			string actorName = requestContext.RouteData.GetRequiredString("Actor");
//
//			if (actorName.ToLowerInvariant() == "simplerequest")
//				return new ActorHttpAsyncHandler<SimpleRequestActor>();
//
//			string virtualPath = "~/Default.aspx";
//
//			return (Page)BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(Page));
		}
	}
}