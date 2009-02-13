namespace Magnum.ActorModel.WebSpecs
{
	using System;
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

					x.InstanceOf<RequestService>()
						.Is.OfConcreteType<RequestService>();
				});

			WithEach<IStartable>(x => x.Start());
		}
	}
}