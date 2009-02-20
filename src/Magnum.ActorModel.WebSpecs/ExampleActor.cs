namespace Magnum.ActorModel.WebSpecs
{
	using System;

	public class ExampleActor
	{
		private long _number;

		public void Act(Action<long> continuation)
		{
			ExampleRequestImpl message = new ExampleRequestImpl(_number, () => Complete, () => Failed);
			
		}

		private void Failed(Exception obj)
		{
			throw new NotImplementedException();
		}

		private void Complete(long result)
		{
			throw new NotImplementedException();
		}
	}


	public class ExampleRequestImpl :
		ExampleRequest
	{
		private readonly Func<Action<Exception>> _complain;
		private readonly Func<Action<long>> _continuation;
		private readonly long _number;

		public ExampleRequestImpl(long number, Func<Action<long>> continuation, Func<Action<Exception>> complain)
		{
			_number = number;
			_continuation = continuation;
			_complain = complain;
		}

		public long Number
		{
			get { return _number; }
		}

		public Action<long> Continue
		{
			get { return _continuation(); }
		}

		public Action<Exception> Complain
		{
			get { return _complain(); }
		}
	}
}