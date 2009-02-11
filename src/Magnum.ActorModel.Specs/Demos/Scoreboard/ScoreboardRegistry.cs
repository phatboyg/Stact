namespace Magnum.ActorModel.Specs.Demos.Scoreboard
{
	using Channels;
	using Messages;
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


			ForRequestedType<Channel<UpdateHighScore>>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.OfConcreteType<ChannelImpl<UpdateHighScore>>();

			ForRequestedType<Channel<string>>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.OfConcreteType<ChannelImpl<string>>();

			ForRequestedType<CommandExecutor>()
				.TheDefault.Is.OfConcreteType<SynchronousCommandExecutor>();

			ForRequestedType<CommandQueue>()
				.TheDefault.Is.OfConcreteType<AsyncCommandQueue>()
				.WithCtorArg("limit").EqualTo(5000)
				.WithCtorArg("waitTime").EqualTo(1000);

			ForRequestedType<CommandContext>()
				.TheDefault.Is.OfConcreteType<ThreadCommandContext>();

			InstanceOf<HighScoreBoard>()
				.Is.OfConcreteType<HighScoreBoard>();
		}
	}
}