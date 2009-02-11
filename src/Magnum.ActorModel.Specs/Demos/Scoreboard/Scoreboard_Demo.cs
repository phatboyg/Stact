namespace Magnum.ActorModel.Specs.Demos.Scoreboard
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Channels;
	using Messages;
	using NUnit.Framework;
	using StructureMap;

	[TestFixture]
	public class Scoreboard_Demo
	{
		#region Setup/Teardown

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

		#endregion

		public static void WithEach<T>(Action<T> action)
		{
			foreach (var configuration in ObjectFactory.Model.PluginTypes)
			{
				if (!configuration.Implements<T>()) continue;

				var instance = (T) ObjectFactory.GetInstance(configuration.PluginType);

				action(instance);
			}
		}

		[Category("Demo")]
		[Explicit]
		[Test]
		public void Demonstrate_a_series_of_players_updating_their_high_scores()
		{
			Trace.WriteLine("Getting startable things");

			WithEach<IStartable>(x => x.Start());

			Player playerA = new Player();

			var channel = ObjectFactory.GetInstance<Channel<UpdateHighScore>>();

			channel.Publish(new UpdateHighScore
			                	{
			                		PlayerId = playerA.Id,
			                		DateAchieved = DateTime.Now,
			                		Score = 10923,
			                	});

			Thread.Sleep(500);

			channel.Publish(new UpdateHighScore
			                	{
			                		PlayerId = playerA.Id,
			                		DateAchieved = DateTime.Now,
			                		Score = 89498,
			                	});


			Thread.Sleep(5000);
		}
	}

	public static class TypeExtensions
	{
		public static bool Implements<T>(this PluginTypeConfiguration configuration)
		{
			return configuration.PluginType.Implements<T>();
		}

		public static bool Implements<T>(this Type type)
		{
			return typeof (T).IsAssignableFrom(type);
		}


		public static void WithEach<T>(this Container container, Action<T> action)
		{
			foreach (var configuration in container.Model.PluginTypes)
			{
				if (!configuration.Implements<T>()) continue;

				var instance = (T) container.GetInstance(configuration.PluginType);

				action(instance);
			}
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