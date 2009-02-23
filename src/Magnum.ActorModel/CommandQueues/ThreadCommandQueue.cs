namespace Magnum.ActorModel.CommandQueues
{
	using System;
	using System.Threading;

	public class ThreadCommandQueue :
		AsyncCommandQueue,
		IStartable
	{
		private readonly Thread _thread;

		public ThreadCommandQueue(int limit, int waitTime) :
			base(limit, waitTime)
		{
			_thread = new Thread(RunThread);
			_thread.Name = "ThreadCommandQueue-" + _thread.ManagedThreadId;
			_thread.IsBackground = false;
			_thread.Priority = ThreadPriority.Normal;
		}

		public Thread Thread
		{
			get { return _thread; }
		}

		public void Start()
		{
			_thread.Start();
		}

		private void RunThread()
		{
			try
			{
				Run();
			}
			catch (Exception)
			{
				//TODO
			}
		}

		public void Join()
		{
			_thread.Join();
		}
	}
}