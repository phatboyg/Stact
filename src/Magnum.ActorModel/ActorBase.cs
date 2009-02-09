namespace Magnum.ActorModel
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	public abstract class ActorBase : IActor
	{
		private readonly List<Action> _actions = new List<Action>();
		private readonly int _commandLimit;
		private readonly int _commandWaitTime;
		private readonly ICommandExecutor _executor;
		private readonly object _lock = new object();
		private bool _enabled = true;

		protected ActorBase(int commandLimit, int commandWaitTime, ICommandExecutor executor)
		{
			_commandLimit = commandLimit;
			_executor = executor;
			_commandWaitTime = commandWaitTime;
		}

		public int CommandLimit
		{
			get { return _commandLimit; }
		}

		public int CommandWaitTime
		{
			get { return _commandWaitTime; }
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
				_enabled = false;
				Monitor.PulseAll(_lock);
			}
		}

		private bool SpaceAvailable(int needed)
		{
			if (!_enabled) return false;

			if (_commandLimit <= 0 || _actions.Count + needed <= _commandLimit)
				return true;

			if (_commandWaitTime <= 0)
				throw new ActorBusyException(_actions.Count);

			Monitor.Wait(_lock, _commandWaitTime);
			if (!_enabled) return false;

			if (_commandLimit > 0 && _actions.Count + needed > _commandLimit)
				throw new ActorBusyException(_actions.Count);

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