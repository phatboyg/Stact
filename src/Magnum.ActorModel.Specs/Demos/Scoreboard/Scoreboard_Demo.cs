namespace Magnum.ActorModel.Specs.Demos.Scoreboard
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading;
	using Channels;
	using Messages;
	using NUnit.Framework;
	using StructureMap;

	[TestFixture]
	public class Scoreboard_Demo
	{
		[Category("Demo")]
		[Explicit]
		[Test]
		public void Demonstrate_a_series_of_players_updating_their_high_scores()
		{
			var container = new Container(new ScoreboardRegistry());

			Trace.WriteLine("Getting startable things");

			var query = container.Model.PluginTypes
				.Where(p => p.IsStartable())
				.Select(x => x.ToStartable());
			foreach (IStartable startable in query)
			{
				Trace.WriteLine("Starting");
				startable.Start();
			}


		
			Player playerA = new Player();


			UpdateHighScore update = new UpdateHighScore
				{
					PlayerId = playerA.Id,
                    DateAchieved = DateTime.Now,
                    Score = 10923,
				};

			container.GetInstance<Channel<UpdateHighScore>>().Publish(update);


			Thread.Sleep(5000);
			
		}
	}

	public static class TypeExtensions
	{
		public static bool IsStartable(this PluginTypeConfiguration configuration)
		{
			return typeof(IStartable).IsAssignableFrom(configuration.PluginType);
		}

		public static IStartable ToStartable(this PluginTypeConfiguration configuration)
		{
			return (IStartable)ObjectFactory.GetInstance(configuration.PluginType);
		}
	}

	public class HighScoreBoard :
		IStartable
	{
		private readonly Channel<UpdateHighScore> _uhs;

		public HighScoreBoard(CommandQueue queue, Channel<UpdateHighScore> uhs) 
		{
			_uhs = uhs;

			_uhs.Subscribe(queue, Consume);
		}

		public void Consume(UpdateHighScore message)
		{
			Trace.WriteLine("High score received for " + message.Score);
		}

		public void Start()
		{
			Trace.WriteLine("Start Called");
		}
	}

	public class Player
	{
		private Guid _id = Guid.NewGuid();

		public Guid Id
		{
			get { return _id; }
		}
	}
}