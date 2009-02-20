namespace Magnum.ActorModel.Specs.Demos.PingPong
{
	using System.Diagnostics;
	using System.Threading;
	using NUnit.Framework;
	using Schedulers;

	[TestFixture]
	public class PingPong_Demo
	{
		private Ping _ping;
		private Pong _pong;
		private Ping _ping2;
		private Ping _ping3;
		private Ping _ping4;

		[SetUp]
		public void Setup()
		{
			_ping = new Ping();
			_ping2 = new Ping();
			_ping3 = new Ping();
			_ping4 = new Ping();
			_pong = new Pong();
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
			ThreadActionScheduler scheduler = new ThreadActionScheduler();
			scheduler.Schedule(0, 1000, ()=>
				{
					Trace.WriteLine("Starting it up");
					_ping.Start(10, _pong);
				});

			Thread.Sleep(6000);
		}
		
	}
}