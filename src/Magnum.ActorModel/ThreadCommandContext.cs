namespace Magnum.ActorModel
{
	using System;
	using System.Threading;
	using Schedulers;

	public class ThreadCommandContext :
		Scheduler,
		IStartable,
		CommandContext
	{
		private readonly CommandQueue _queue;
		private readonly Thread _thread;
		private ActionScheduler _scheduler;

		public ThreadCommandContext(CommandQueue queue)
		{
			_queue = queue;
			_thread = new Thread(RunThread);
			_thread.Name = string.Format("ThreadCommandContext-{0}", _thread.ManagedThreadId);
			_thread.IsBackground = false;
			_thread.Priority = ThreadPriority.Normal;

			_scheduler = new ActionScheduler(this);
		}

		public Thread Thread
		{
			get { return _thread; }
		}

		public void Dispose()
		{
			_queue.Disable();
		}

		private void RunThread()
		{
			try
			{
				_queue.Run();
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

		public void Enqueue(Action action)
		{
			_queue.Enqueue(action);
		}

		public void EnqueueAll(params Action[] actions)
		{
			_queue.EnqueueAll(actions);
		}

		public void Disable()
		{
			_queue.Disable();
		}

		public void Run()
		{
			throw new NotImplementedException();
		}

		public void Start()
		{
			_thread.Start();
		}

		public Unschedule Schedule(int interval, Action action)
		{
			return _scheduler.Schedule(interval, action);
		}

		public Unschedule Schedule(int initialInterval, int periodicInterval, Action action)
		{
			return _scheduler.Schedule(initialInterval, periodicInterval, action);
		}
	}

	public interface IStartable
	{
		void Start();
	}
}