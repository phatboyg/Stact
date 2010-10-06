// Copyright 2010 Chris Patterson
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Stact.Specs.Actors
{
	using System.Diagnostics;
	using Fibers;
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
		private readonly Fiber _fiber;

		protected ActorImpl()
		{
			_fiber = new PoolFiber();
		}

		public void Call<T>(IActor caller, T method)
		{
			_fiber.Add(() => Dispatch(caller, method));
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