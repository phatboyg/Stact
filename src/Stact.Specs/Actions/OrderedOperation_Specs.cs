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
namespace Stact.Specs.Actions
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Internal;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using NUnit.Framework;


	[Scenario]
	public class When_adding_operations_to_a_fiber
	{
		int _count;
		int[] _values;

		[When]
		public void Adding_operations_to_a_fiber()
		{
			_count = 10000;
			_values = new int[_count];
			Fiber fiber = new PoolFiber();

			int index = 0;
			var completed = new Future<int>();

			var go = new Future<bool>();

			fiber.Add(() => { go.WaitUntilCompleted(10.Seconds()); });

			for (int i = 0; i < _count; i++)
			{
				int offset = i;
				fiber.Add(() =>
					{
						_values[offset] = index++;

						if (offset == _count - 1)
							completed.Complete(offset);
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
				if (_values[i] != i)
					Assert.Fail("Order not correct");
			}
		}
	}
}