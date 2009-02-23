namespace Magnum.ActorModel.CommandQueues
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
		private readonly object _lock = new object();
		private bool _enabled = true;

		public AsyncCommandQueue(int limit, int waitTime)
		{
			_commandLimit = limit;
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
				_enabled = false;
				Monitor.PulseAll(_lock);
			}
		}

		private Action[] DequeueAll()
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

		public bool ActionsAvailable()
		{
			while (_actions.Count == 0 && _enabled)
			{
				Monitor.Wait(_lock);
			}

			return _enabled;
		}

		public bool ExecuteAvailableActions()
		{
			Action[] actions = DequeueAll();
			if (actions == null)
				return false;

			foreach (var action in actions)
			{
				if (_enabled == false)
					break;

				action();
			}

			return true;
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
	}
}