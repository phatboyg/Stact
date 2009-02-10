namespace Magnum.ActorModel.Specs.Demos.Scoreboard
{
	using Channels;
	using StructureMap.Attributes;
	using StructureMap.Configuration.DSL;

	public class ScoreboardRegistry :
		Registry
	{
		public ScoreboardRegistry()
		{
			ForRequestedType(typeof (Channel<>))
				.CacheBy(InstanceScope.Singleton)
				.TheDefaultIsConcreteType(typeof (ChannelImpl<>));

			ForRequestedType<CommandExecutor>()
				.TheDefault.Is.OfConcreteType<SynchronousCommandExecutor>();

			ForRequestedType<CommandQueue>()
				.TheDefault.Is.OfConcreteType<SingleThreadedCommandQueue>()
				.CtorDependency<CommandQueue>().Is(x => x
					.OfConcreteType<AsyncCommandQueue>()
					.WithCtorArg("limit").EqualTo(5000)
					.WithCtorArg("waitTime").EqualTo(1000));

			ForRequestedType<HighScoreBoard>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.OfConcreteType<HighScoreBoard>()
				.CtorDependency<CommandQueue>("queue").Is(x => x.OfConcreteType<SingleThreadedCommandQueue>().CtorDependency<CommandQueue>().Is(y => y.OfConcreteType<AsyncCommandQueue>()
					.WithCtorArg("limit").EqualTo(5000)
					.WithCtorArg("waitTime").EqualTo(1000)));
		}
	}
}