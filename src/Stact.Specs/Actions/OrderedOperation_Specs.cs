namespace Stact.Specs.Actions
{
	using Internal;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[Scenario]
	public class When_adding_operations_to_a_fiber
	{
		int[] _values;
		int _count;

		[When]
		public void Adding_operations_to_a_fiber()
		{
			_count = 10000;
			_values = new int[_count];
			Fiber fiber = new PoolFiber();

			int index = 0;
			Future<int> completed = new Future<int>();

			Future<bool> go = new Future<bool>();

			fiber.Add(() =>
				{
					go.WaitUntilCompleted(10.Seconds());
				});

			for (int i = 0; i < _count; i++)
			{
				int offset = i;
				fiber.Add(() =>
					{
						_values[offset] = index++;

						if (offset == _count - 1)
						{
							completed.Complete(offset);
						}
					});
			}

			go.Complete(true);

			completed.WaitUntilCompleted(10.Seconds()).ShouldBeTrue();
		}


		[Then]
		public void Should_execute_them_in_order()
		{
			for (int i = 0; i < _count; i++)
			{
				if(_values[i] != i)
					Assert.Fail("Order not correct");
			}
		}
	}
}