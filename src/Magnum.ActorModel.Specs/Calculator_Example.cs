namespace Magnum.ActorModel.Specs
{
	using System.Diagnostics;
	using CommandQueues;
	using NUnit.Framework;

	[TestFixture]
	public class Calculator_Example
	{
		[Test]
		public void I_want_to_make_this_easier()
		{
			var testor = new TestActor();

			var calculator = new CalculatorActor();

			calculator.Call(testor, new Calculator.Add(27.0m));
		}
	}

	public class TestActor :
		ActorImpl
	{
		
	}

	public class Calculator
	{
		public class ValueChanged
		{
			private readonly decimal _value;

			public ValueChanged(decimal value)
			{
				_value = value;
			}

			public decimal Value
			{
				get { return _value; }
			}
		}

		public class Add
		{
			private readonly decimal _value;

			public Add(decimal value)
			{
				_value = value;
			}

			public decimal Value
			{
				get { return _value; }
			}
		}
	}


	public interface IActor
	{
		void Call<T>(IActor caller, T method);
	}

	public abstract class ActorImpl : IActor
	{
		private readonly ThreadPoolCommandQueue _queue;

		protected ActorImpl()
		{
			_queue = new ThreadPoolCommandQueue();
		}

		public void Call<T>(IActor caller, T method)
		{
			_queue.Enqueue(() => Dispatch(caller, method));
		}

		public void Dispatch<T>(IActor caller, T method)
		{
			Trace.WriteLine("Dispatching: " + typeof (T));
		}
	}

	public class CalculatorActor :
		ActorImpl
	{
		private decimal _value;

		private void Add(Calculator.Add add)
		{
			if (add.Value == 0.0m)
				return;

			_value += add.Value;

			
		}
	}
}