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
	using Magnum.Actors.CommandQueues;
	using Magnum.Actors.Exceptions;
	using MbUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class CommandQueue_Specs
	{
		[Test]
		public void The_queue_should_throw_an_exception_if_there_is_no_room_for_new_commands()
		{
			var queue = new AsyncCommandQueue(2, 0);
			queue.Enqueue(delegate { });
			queue.Enqueue(delegate { });

			try
			{
				queue.Enqueue(delegate { });
				Assert.Fail("failed");
			}
			catch (QueueFullException failed)
			{
				Assert.AreEqual(2, failed.Depth);
				Assert.AreEqual("The queue is full and cannot accept new commands (2)", failed.Message);
			}
		}

		[Test]
		public void The_queue_should_not_hide_any_exceptions_from_the_caller()
		{
			var action = MockRepository.GenerateMock<Action>();
			var exception = new Exception();

			action.Expect(x => x()).Throw(exception);

			var queue = new AsyncCommandQueue(100, 100);
			queue.Enqueue(action);

			try
			{
				queue.ExecuteAvailableActions();

				Assert.Fail("Should throw Exception");
			}
			catch (Exception ex)
			{
				Assert.AreSame(exception, ex);
			}

			action.VerifyAllExpectations();
		}

		[Test]
		public void The_queue_should_only_execute_actions_while_enabled()
		{
			var first = MockRepository.GenerateMock<Action>();
			var second = MockRepository.GenerateMock<Action>();
			var third = MockRepository.GenerateMock<Action>();

			var queue = new AsyncCommandQueue(100, 100);
			queue.Enqueue(first);

			var run = new Thread(queue.Run) {IsBackground = true};

			run.Start();
			Thread.Sleep(100);
			queue.Enqueue(second);
			queue.Disable();
			queue.Enqueue(third);
			Thread.Sleep(100);
			run.Join();

			first.AssertWasCalled(x => x());
			second.AssertWasCalled(x => x());
			third.AssertWasNotCalled(x => x());
		}
	}
}