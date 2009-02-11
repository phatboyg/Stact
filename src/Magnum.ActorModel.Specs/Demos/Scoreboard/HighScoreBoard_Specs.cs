namespace Magnum.ActorModel.Specs.Demos.Scoreboard
{
	using Channels;
	using NUnit.Framework;
	using StructureMap;

	[TestFixture]
	public class HighScoreBoard_Specs
	{
		[SetUp]
		public void Setup()
		{
			ObjectFactory.Initialize(x => x.AddRegistry(new ScoreboardRegistry()));
		}

		[TearDown]
		public void Teardown()
		{
			ObjectFactory.ResetDefaults();
		}

		[Test]
		public void FIRST_TEST_NAME()
		{
			var board = ObjectFactory.GetInstance<HighScoreBoard>();

			board.Start();
		}

		[Test]
		public void Channels_should_be_the_same()
		{
			var channel1 = ObjectFactory.GetInstance<Channel<string>>();
			var channel2 = ObjectFactory.GetInstance<Channel<string>>();

			Assert.IsTrue(ReferenceEquals(channel1, channel2));
			
		}

	}
}