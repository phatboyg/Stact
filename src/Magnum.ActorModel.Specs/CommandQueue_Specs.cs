namespace Magnum.ActorModel.Specs
{
	using System;
	using System.Threading;
	using CommandQueues;
	using Exceptions;
	using NUnit.Framework;
	using Rhino.Mocks;

	[TestFixture]
	public class CommandQueue_Specs
	{
		[Test]
		public void The_queue_should_throw_an_exception_if_there_is_no_room_for_new_commands()
		{
			var queue = new AsyncCommandQueue(2, 0, new SynchronousCommandExecutor());
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

			var queue = new AsyncCommandQueue(100, 100, new SynchronousCommandExecutor());
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

			var queue = new AsyncCommandQueue(100, 100, new SynchronousCommandExecutor());
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