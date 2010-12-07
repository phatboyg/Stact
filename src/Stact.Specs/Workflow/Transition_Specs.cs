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
namespace Stact.Specs.Workflow
{
	using Magnum.TestFramework;
	using Stact.Workflow;


	[Scenario]
	public class When_creating_a_new_workflow
	{
		WorkflowInstance<TestWorkflow> _instance;
		StateMachineWorkflow<TestWorkflow, TestInstance> _workflow;

		[When]
		public void Creating_a_new_workflow()
		{
			_workflow = StateMachineWorkflow.New<TestWorkflow, TestInstance>(x =>
				{
					x.AccessCurrentState(y => y.CurrentState);
				});

			_instance = _workflow.GetInstance(new TestInstance());
		}

		[Then]
		public void Initial_state_should_be_found()
		{
			State initial = _workflow.GetState("Initial");

			initial.ShouldNotBeNull();
			initial.Name.ShouldEqual("Initial");
		}

		[Then]
		public void Initial_state_should_be_automatically_created()
		{
			State current = _instance.CurrentState;

			current.Name.ShouldEqual("Initial");
		}


		class TestInstance
		{
			public State CurrentState { get; set; }
		}


		interface TestWorkflow
		{
		}
	}


	[Scenario]
	public class When_an_event_causing_a_state_transition
	{
		WorkflowInstance<TestWorkflow> _instance;
		StateMachineWorkflow<TestWorkflow, TestInstance> _workflow;

		[When]
		public void An_event_causing_a_state_transition()
		{
			_workflow = StateMachineWorkflow.New<TestWorkflow, TestInstance>(x =>
				{
					x.AccessCurrentState(y => y.CurrentState);

					x.During(y => y.Initial)
						.When(y => y.Finish)
						.TransitionTo(y => y.Completed);
				});

			_instance = _workflow.GetInstance(new TestInstance());

			_instance.RaiseEvent(x => x.Finish);
		}

		[Then]
		public void Should_result_in_the_target_state()
		{
			State current = _instance.CurrentState;

			current.ShouldEqual(_workflow.GetState(x => x.Completed));
		}

		class TestInstance
		{
			public State CurrentState { get; set; }
		}


		interface TestWorkflow
		{
			State Initial { get; }
			State Completed { get; }

			Event Finish { get; }
		}
	}

	[Scenario]
	public class When_a_message_event_changes_the_state
	{
		WorkflowInstance<TestWorkflow> _instance;
		StateMachineWorkflow<TestWorkflow, TestInstance> _workflow;

		[When]
		public void A_message_event_changes_the_state()
		{
			_workflow = StateMachineWorkflow.New<TestWorkflow, TestInstance>(x =>
				{
					x.AccessCurrentState(y => y.CurrentState);

					x.During(y => y.Initial)
						.When(y => y.Finish)
						.TransitionTo(y => y.Completed);
				});

			_instance = _workflow.GetInstance(new TestInstance());

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

		class TestInstance
		{
			public State CurrentState { get; set; }
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
	public class When_raising_multiple_events
	{
		WorkflowInstance<TestWorkflow> _instance;
		StateMachineWorkflow<TestWorkflow, TestInstance> _workflow;

		[When]
		public void Raising_multiple_events()
		{
			_workflow = StateMachineWorkflow.New<TestWorkflow, TestInstance>(x =>
				{
					x.AccessCurrentState(y => y.CurrentState);

					x.During(y => y.Initial)
						.When(y => y.Start)
						.TransitionTo(y => y.Running);

					x.During(y => y.Running)
						.When(y => y.Finish)
						.TransitionTo(y => y.Completed);
				});

			_instance = _workflow.GetInstance(new TestInstance());

			_instance.RaiseEvent(x => x.Start);
			_instance.RaiseEvent(x => x.Finish);
		}

		[Then]
		public void Should_result_in_the_target_state()
		{
			State current = _instance.CurrentState;

			current.ShouldEqual(_workflow.GetState(x => x.Completed));
		}

		class TestInstance
		{
			public State CurrentState { get; set; }
		}


		interface TestWorkflow
		{
			State Initial { get; }
			State Running { get; }
			State Completed { get; }

			Event Start { get; }
			Event Finish { get; }
		}


		/*
	
		[Test]
		public void Typed_events_should_carry_their_data_to_the_expression()
		{
			ExampleStateMachine example = new ExampleStateMachine();

			example.SubmitCommentCard(new CommentCard { IsComplaint = true });

			Assert.AreEqual(ExampleStateMachine.WaitingForManager, example.CurrentState);
		}

		[Test]
		public void Multiple_expressions_per_event_should_run_in_order()
		{
			ExampleStateMachine example = new ExampleStateMachine();

			example.SubmitCommentCard(new CommentCard { IsComplaint = true });

			Assert.AreEqual(ExampleStateMachine.WaitingForManager, example.CurrentState);

			example.BurnCommentCard();

			Assert.AreEqual(ExampleStateMachine.Completed, example.CurrentState);
		}

		[Test]
		public void Typed_events_should_carry_their_data_to_the_expression_other()
		{
			ExampleStateMachine example = new ExampleStateMachine();

			example.SubmitCommentCard(new CommentCard { IsComplaint = false });

			Assert.AreEqual(ExampleStateMachine.Completed, example.CurrentState);
		}
*/
	}
}