namespace Magnum.ActorModel.Specs.Demos.Scoreboard
{
	using Channels;
	using CommandQueues;
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
				.TheDefaultIsConcreteType(typeof (SynchronousChannel<>));


			ForRequestedType<Channel<UpdateHighScore>>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.OfConcreteType<SynchronousChannel<UpdateHighScore>>();

			ForRequestedType<Channel<string>>()
				.CacheBy(InstanceScope.Singleton)
				.TheDefault.Is.OfConcreteType<SynchronousChannel<string>>();

			ForRequestedType<CommandQueue>()
				.TheDefault.Is.OfConcreteType<AsyncCommandQueue>()
				.WithCtorArg("limit").EqualTo(5000)
				.WithCtorArg("waitTime").EqualTo(1000);

			InstanceOf<HighScoreBoard>()
				.Is.OfConcreteType<HighScoreBoard>();
		}
	}
}