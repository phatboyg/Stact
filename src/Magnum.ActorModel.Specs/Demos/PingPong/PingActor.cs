namespace Magnum.ActorModel.Specs.Demos.PingPong
{
	using System.Diagnostics;
	using System.Threading;
	using CommandQueues;

	public class PingActor :
		Actor, Ping
	{
		private int _initialCount;
		private int _pingCount;
		private Stopwatch _watch;
		protected CommandQueue _queue;

		public PingActor()
		{
			_queue = new ThreadPoolCommandQueue();
		}

		public void Start(int count, Pong pong)
		{
			_initialCount = _pingCount = count;
			_watch = Stopwatch.StartNew();

			pong.Ping(this);
		}

		public void Pong(Pong pong)
		{
			_queue.Enqueue(() => Consume(pong));
		}

		private void Consume(Pong pong)
		{
			_pingCount--;
			if (_pingCount == 0)
			{
				_watch.Stop();
				Trace.WriteLine("Ping -> Pong " + _pingCount + " On Thread: " + Thread.CurrentThread.ManagedThreadId);

				long perSecond = ((long) _initialCount*2000)/(_watch.ElapsedMilliseconds == 0 ? 1 : _watch.ElapsedMilliseconds);
				Trace.WriteLine("Elapsed Time = " + _watch.ElapsedMilliseconds + "ms, Messages Per Second = " + perSecond);
				return;
			}

			if (_pingCount%50000 == 0)
			{
				Trace.WriteLine("Ping -> Pong " + _pingCount + " On Thread: " + Thread.CurrentThread.ManagedThreadId);
			}

			pong.Ping(this);
		}
	}
}