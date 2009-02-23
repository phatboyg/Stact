namespace Magnum.ActorModel.Specs.Demos.PingPong
{
	using System.Diagnostics;
	using System.Threading;
	using NUnit.Framework;
	using Schedulers;

	[TestFixture]
	public class PingPong_Demo
	{
		private PingActor _ping;
		private PingActor _ping2;
		private PingActor _ping3;
		private PingActor _ping4;
		private Pong _pong;

		[SetUp]
		public void Setup()
		{
			_ping = new PingActor();
			_ping2 = new PingActor();
			_ping3 = new PingActor();
			_ping4 = new PingActor();
			_pong = new PongActor();
		}

		[Test, Explicit, Category("Demo")]
		public void Demo()
		{
			_ping.Start(10, _pong);
			Thread.Sleep(1000);

			_ping.Start(400000, _pong);
			_ping2.Start(400000, _pong);
			_ping3.Start(400000, _pong);
			_ping4.Start(400000, _pong);

			Thread.Sleep(30000);
		}

		[Test, Explicit, Category("Demo")]
		public void Timer_Based_Demo()
		{
			ThreadPoolScheduler scheduler = new ThreadPoolScheduler();
			scheduler.Schedule(0, 1000, ()=>
				{
					Trace.WriteLine("Starting it up");
					_ping.Start(10, _pong);
				});

			Thread.Sleep(6000);
		}
		
	}
}