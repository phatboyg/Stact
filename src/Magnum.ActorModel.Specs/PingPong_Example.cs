namespace Magnum.ActorModel.Specs
{
	using System.Diagnostics;
	using System.Threading;
	using CommandQueues;
	using NUnit.Framework;

	[TestFixture]
	public class PingPong_Example
	{
		[SetUp]
		public void Setup()
		{
			_jack = new PingPongPlayer("Jack").GetProxy();
			_jill = new PingPongPlayer("Jill").GetProxy();
		}

		[Test, Explicit]
		public void Show_the_ping_pong_blowing_the_stack()
		{
			_jack.PingPong(1000000, _jill);

			Thread.Sleep(10000);
		}

		[Test]
		public void Show_the_ping_pong_going_back_and_forth()
		{
			_jack.PingPong(1000, _jill);

			Thread.Sleep(10000);
		}

		private IPingPongPlayer _jack;
		private IPingPongPlayer _jill;
	}

	public interface IPingPongPlayer
	{
		void PingPong(int count, IPingPongPlayer partner);
	}

	public class PingPongPlayer : IPingPongPlayer
	{
		private readonly string _name;
		private IPingPongPlayer _actorProxy;
		private CommandQueue _queue = new ThreadPoolCommandQueue();
		private Stopwatch _watch = Stopwatch.StartNew();

		public PingPongPlayer(string name)
		{
			_name = name;

			_queue.Run();
			_actorProxy = new PingPongPlayerProxy(this);
		}

		public void PingPong(int count, IPingPongPlayer partner)
		{
			if (count == 0)
			{
				_watch.Stop();
				Trace.WriteLine(_name + ": done in " + _watch.ElapsedMilliseconds + "ms");

				return;
			}

			if (count%5000 == 0 || count%5000 == 1)
			{
				//Trace.WriteLine(_name + ": PingPong = " + pingPongParams.Count + " On Thread: " + Thread.CurrentThread.ManagedThreadId);
			}

			partner.PingPong(count - 1, _actorProxy);
		}

		public IPingPongPlayer GetProxy()
		{
			return _actorProxy;
		}

		public class PingPongPlayerProxy : IPingPongPlayer
		{
			private readonly PingPongPlayer _player;

			public PingPongPlayerProxy(PingPongPlayer player)
			{
				_player = player;
			}

			public void PingPong(int count, IPingPongPlayer partner)
			{
				_player._queue.Enqueue(() => _player.PingPong(count, partner));
			}
		}
	}
}