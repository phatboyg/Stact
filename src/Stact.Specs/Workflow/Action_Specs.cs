namespace Stact.Specs.Workflow
{
	using Magnum.TestFramework;
	using Stact.Workflow;


	[Scenario]
	public class When_a_message_event_is_raised
	{
		WorkflowInstance<TestWorkflow> _instance;
		StateMachineWorkflow<TestWorkflow, TestInstance> _workflow;
		TestInstance _testInstance;

		[When]
		public void A_message_event_is_raised()
		{
			_workflow = StateMachineWorkflow.New<TestWorkflow, TestInstance>(x =>
			{
				x.AccessCurrentState(y => y.CurrentState);

				x.During(y => y.Initial)
					.When(y => y.Finish)
					.Then((instance, message) => instance.ResultValue = message.Value)
					.TransitionTo(y => y.Completed);
			});

			_testInstance = new TestInstance();

			_instance = _workflow.GetInstance(_testInstance);

			_instance.RaiseEvent(x => x.Finish, new Result
			{
				Value = "Success"
			});
		}

		[Then]
		public void Should_result_in_the_target_state()
		{
			State current = _instance.CurrentState;

			current.ShouldEqual(_workflow.GetState(x => x.Completed));
		}

		[Then]
		public void Should_contain_the_result_in_the_instance_property()
		{
			_testInstance.ResultValue.ShouldEqual("Success");
		}

		class TestInstance
		{
			public State CurrentState { get; set; }

			public string ResultValue { get; set; }
		}

		class Result
		{
			public string Value { get; set; }
		}

		interface TestWorkflow
		{
			State Initial { get; }
			State Completed { get; }

			Event<Result> Finish { get; }
		}
	}

	[Scenario]
	public class When_a_simple_event_is_raised
	{
		WorkflowInstance<TestWorkflow> _instance;
		StateMachineWorkflow<TestWorkflow, TestInstance> _workflow;
		TestInstance _testInstance;

		[When]
		public void A_simple_event_is_raised()
		{
			_workflow = StateMachineWorkflow.New<TestWorkflow, TestInstance>(x =>
			{
				x.AccessCurrentState(y => y.CurrentState);

				x.During(y => y.Initial)
					.When(y => y.Finish)
					.Then(instance => instance.ResultValue = "Success")
					.TransitionTo(y => y.Completed);
			});

			_testInstance = new TestInstance();

			_instance = _workflow.GetInstance(_testInstance);

			_instance.RaiseEvent(x => x.Finish);
		}

		[Then]
		public void Should_result_in_the_target_state()
		{
			State current = _instance.CurrentState;

			current.ShouldEqual(_workflow.GetState(x => x.Completed));
		}

		[Then]
		public void Should_contain_the_result_in_the_instance_property()
		{
			_testInstance.ResultValue.ShouldEqual("Success");
		}

		class TestInstance
		{
			public State CurrentState { get; set; }

			public string ResultValue { get; set; }
		}

		interface TestWorkflow
		{
			State Initial { get; }
			State Completed { get; }

			Event Finish { get; }
		}
	}
}