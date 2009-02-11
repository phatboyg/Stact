namespace Magnum.ActorModel.Specs
{
	using System;
	using System.Threading;
	using NUnit.Framework;
	using Rhino.Mocks;
	using Schedulers;

	[TestFixture]
	public class ActionTimer_Specs
	{
        [Test]
        public void Cancelling_the_action_should_prevent_it_from_being_called()
        {
            var executionCount = 0;
            Action action = () => executionCount++;
            var timer = new TimerAction(1, 2, action);
            timer.ExecuteAction();
            Assert.AreEqual(1, executionCount);
            timer.Cancel();
        	timer.ExecuteAction();
            Assert.AreEqual(1, executionCount);
        }

        [Test]
        public void A_recurring_event_should_reschedule_itself_with_the_controller()
        {
            var action = MockRepository.GenerateMock<Action>();
            var timer = new TimerAction(2, 3, action);
            var control = MockRepository.GenerateMock<SchedulerControl>();

        	timer.TimerIntervalCallback(control);

        	control.AssertWasCalled(x => x.Enqueue(timer.ExecuteAction));
        }

        [Test]
        public void A_recurring_event_should_remove_itself_from_the_controller()
        {
			var action = MockRepository.GenerateMock<Action>();
			var timer = new TimerAction(2, 3, action);
			var control = MockRepository.GenerateMock<SchedulerControl>();

			timer.Cancel();
			timer.TimerIntervalCallback(control);

			control.AssertWasCalled(x => x.Remove(timer));
        }

        [Test]
        public void A_one_shot_event_should_remove_itself_from_the_controller()
        {
			var action = MockRepository.GenerateMock<Action>();
			var timer = new TimerAction(2, Timeout.Infinite, action);
			var control = MockRepository.GenerateMock<SchedulerControl>();

			timer.TimerIntervalCallback(control);

			control.AssertWasCalled(x => x.Remove(timer));
			control.AssertWasCalled(x => x.Enqueue(timer.ExecuteAction));
        }
	}
}