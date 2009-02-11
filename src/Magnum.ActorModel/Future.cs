namespace Magnum.ActorModel
{
	using System;
	using System.Diagnostics;
	using System.Threading;

	public class Future<T>
	{
		private readonly Stopwatch _elapsed = Stopwatch.StartNew();
		private readonly ManualResetEvent _completed = new ManualResetEvent(false);

		public T Value { get; private set; }

		public void Complete(T value)
		{
			_elapsed.Stop();

			Trace.WriteLine("Value Received After " + _elapsed.Elapsed);

			Value = value;
			_completed.Set();
		}

		public bool IsAvailable(TimeSpan timeout)
		{
			return _completed.WaitOne(timeout, true);
		}
	}
}