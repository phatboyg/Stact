// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Specs.Actors
{
	using System;
	using System.Threading;
	using Fibers;
	using NUnit.Framework;

	[TestFixture]
	public class ThreadActionScheduler_Specs
	{
		[Test]
		public void Schedule()
		{
			var fiber = new SynchronousFiber();

			var count = 0;
			var reset = new AutoResetEvent(false);
			Action one = () => Assert.AreEqual(0, count++);
			Action two = () => Assert.AreEqual(1, count++);
			Action three = delegate
				{
					Assert.AreEqual(2, count++);
					reset.Set();
				};

			var thread = new TimerScheduler(fiber);
			thread.Schedule(50, fiber, three);
			thread.Schedule(1, fiber, two);
			thread.Schedule(1, fiber, two);
			Assert.IsTrue(reset.WaitOne(10000, false));
		}

		[Test]
		public void Schedule1000In1ms()
		{
			var fiber = new SynchronousFiber();

			var count = 0;
			var reset = new AutoResetEvent(false);
			Action one = delegate
				{
					count++;
					if (count == 1000)
					{
						reset.Set();
					}
				};

			var thread = new TimerScheduler(fiber);
			{
				for (var i = 0; i < 1000; i++)
				{
					thread.Schedule(i, fiber,one);
				}
				Assert.IsTrue(reset.WaitOne(1200, false));
			}
		}
	}
}