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
	using System.Linq;
	using Internal;
	using Magnum.TestFramework;
	using Stact.Workflow;


	[Scenario]
	public class Visiting_a_workflow_of_an_actor
	{
		StateMachineWorkflowEventVisitor<MyActorWorkflow, MyActor> _visitor;
		StateMachineWorkflow<MyActorWorkflow, MyActor> _workflow;

		[When]
		public void Should_include_all_the_state_events()
		{
			_workflow = StateMachineWorkflow.New<MyActorWorkflow, MyActor>(x =>
				{
					x.AccessCurrentState(i => i.CurrentState);

					x.Initially()
						.When(w => w.Start)
						.TransitionTo(y => y.Running)
						.When(y => y.Stop)
						.TransitionTo(y => y.Stopped);

					x.During(y => y.Running)
						.When(y => y.AReceived)
						.Then(i => i.OnA)
						.When(y => y.BReceived)
						.Then(i => i.OnB)
						.When(y => y.Stop)
						.TransitionTo(y => y.Stopped);

					x.During(y => y.Stopped)
						.When(y => y.Start)
						.TransitionTo(y => y.Running)
						.When(y => y.OnExit)
						.Finalize();
				});

			_visitor = new StateMachineWorkflowEventVisitor<MyActorWorkflow, MyActor>();
			_workflow.Accept(_visitor);
		}

		[Then]
		public void Should_have_three_message_events()
		{
			_visitor.GetBinders().Count().ShouldEqual(3);
		}

		[Then]
		public void Should_only_accept_a_while_running()
		{
			StateMachineWorkflowEventBinder<MyActor> eevent = _visitor.GetBinders()
				.Where(x => x.BodyType == typeof(A))
				.Single();

			eevent.ReceiveStates.Count().ShouldEqual(1);
			eevent.ReceiveStates.Any(s => s == _workflow.GetState(x => x.Running)).ShouldBeTrue();
		}

		[Then]
		public void Should_only_accept_b_while_running()
		{
			StateMachineWorkflowEventBinder<MyActor> eevent = _visitor.GetBinders()
				.Where(x => x.BodyType == typeof(B))
				.Single();

			eevent.ReceiveStates.Count().ShouldEqual(1);
			eevent.ReceiveStates.Any(s => s == _workflow.GetState(x => x.Running)).ShouldBeTrue();
		}


		interface A
		{
		}


		interface B
		{
		}


		class MyActor :
			Actor
		{
			public State CurrentState { get; set; }

			public void OnA(A message)
			{
			}

			public void OnB(B message)
			{
			}
		}


		interface MyActorWorkflow
		{
			Event Start { get; }
			Event Stop { get; }

			Event<A> AReceived { get; }
			Event<B> BReceived { get; }

			Event<Request<Exit>> OnExit { get; }

			State Running { get; }
			State Stopped { get; }
		}
	}
}