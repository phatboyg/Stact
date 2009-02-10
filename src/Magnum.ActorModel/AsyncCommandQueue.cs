namespace Magnum.ActorModel
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using Exceptions;

	public class AsyncCommandQueue :
		CommandQueue
	{
		private readonly List<Action> _actions = new List<Action>();
		private readonly int _commandLimit;
		private readonly int _enqueueWaitTime;
		private readonly CommandExecutor _executor;
		private readonly object _lock = new object();
		private bool _enabled = true;

		public AsyncCommandQueue(int limit, int waitTime, CommandExecutor executor)
		{
			_commandLimit = limit;
			_executor = executor;
			_enqueueWaitTime = waitTime;
		}

		public int CommandLimit
		{
			get { return _commandLimit; }
		}

		public int EnqueueWaitTime
		{
			get { return _enqueueWaitTime; }
		}

		public void Enqueue(Action action)
		{
			lock (_lock)
			{
				if (!SpaceAvailable(1)) return;

				_actions.Add(action);
				Monitor.PulseAll(_lock);
			}
		}

		public void EnqueueAll(params Action[] actions)
		{
			lock (_lock)
			{
				if (!SpaceAvailable(actions.Length)) return;

				_actions.AddRange(actions);
				Monitor.PulseAll(_lock);
			}
		}

		public Action[] DequeueAll()
		{
			lock (_lock)
			{
				if (ActionsAvailable())
				{
					Action[] results = _actions.ToArray();

					_actions.Clear();
					Monitor.PulseAll(_lock);

					return results;
				}
				return null;
			}
		}

		public bool ExecuteAvailableActions()
		{
			Action[] toExecute = DequeueAll();
			if (toExecute == null)
				return false;

			_executor.ExecuteAll(toExecute);
			return true;
		}

		public void Run()
		{
			while (ExecuteAvailableActions())
			{
			}
		}

		public void Disable()
		{
			lock (_lock)
			{
				_executor.Disable();
				_enabled = false;
				Monitor.PulseAll(_lock);
			}
		}

		private bool SpaceAvailable(int needed)
		{
			if (!_enabled) return false;

			if (_commandLimit <= 0 || _actions.Count + needed <= _commandLimit)
				return true;

			if (_enqueueWaitTime <= 0)
				throw new QueueFullException(_actions.Count);

			Monitor.Wait(_lock, _enqueueWaitTime);
			if (!_enabled) return false;

			if (_commandLimit > 0 && _actions.Count + needed > _commandLimit)
				throw new QueueFullException(_actions.Count);

			return true;
		}

		private bool ActionsAvailable()
		{
			while (_actions.Count == 0 && _enabled)
			{
				Monitor.Wait(_lock);
			}

			return _enabled;
		}
	}
}