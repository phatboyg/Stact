namespace Magnum.ActorModel
{
	using System;
	using System.Threading;
	using Exceptions;
	using Schedulers;

	public class PooledCommandContext :
		CommandContext,
		IStartable
	{
		private readonly CommandQueue _queue;
		private ActionScheduler _scheduler;

		public PooledCommandContext(CommandQueue queue)
		{
			_queue = queue;
			_scheduler = new ActionScheduler(this);
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
		}

		public void Dispose()
		{
			_scheduler.Dispose();
			_queue.Disable();
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

	public interface IThreadPool
	{
		void Queue(WaitCallback callback);
	}

	public class DefaultThreadPool :
		IThreadPool
	{
		public void Queue(WaitCallback callback)
		{
			if (!ThreadPool.QueueUserWorkItem(callback))
			{
				throw new QueueFullException("Unable to add item to pool: " + callback.Target);
			}
		}
	}
}