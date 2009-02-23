namespace Magnum.ActorModel.Specs.Demos.PingPong
{
	using CommandQueues;

	public class PongActor :
		Actor, Pong
	{
		private int _pingCount;
		protected CommandQueue _queue;

		public PongActor()
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