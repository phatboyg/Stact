namespace Magnum.ActorModel.Specs.Demos.PingPong
{
	using CommandQueues;

	public class Pong :
		Actor
	{
		private int _pingCount;
		protected CommandQueue _queue;

		public Pong()
		{
			_queue = new ThreadPoolCommandQueue();
		}

		public void Ping(Ping ping)
		{
			_queue.Enqueue(() => Consume(ping));
		}

		private void Consume(Ping ping)
		{
			ping.Pong(this);
			_pingCount++;
		}
	}
}