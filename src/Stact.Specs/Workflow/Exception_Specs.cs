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
	using System;
	using Magnum.TestFramework;
	using Stact.Workflow;


	[Scenario]
	public class When_throwing_an_exception_from_an_event_handler
	{
		bool _handlerCalled;
		WorkflowInstance<SubjectWorkflow> _instance;
		bool _instanceHandlerCalled;
		Subject _subject;
		StateMachineWorkflow<SubjectWorkflow, Subject> _workflow;

		[When]
		public void Throwing_an_exception_from_an_event_handler()
		{
			_workflow = StateMachineWorkflow.New<SubjectWorkflow, Subject>(x =>
				{
					x.AccessCurrentState(y => y.CurrentState);

					x.Initially()
						.When(e => e.Create)
						.Then(i => i.Create)
						.TransitionTo(s => s.Created)
						.InCaseOf()
						.Exception<SubjectException>()
						.Then(i =>
							{
								_instanceHandlerCalled = true;
							})
						.Then(() => _handlerCalled = true)
						.TransitionTo(s => s.Failed);
				});

			_subject = new Subject();

			_instance = _workflow.GetInstance(_subject);

			_instance.RaiseEvent(x => x.Create);
		}

		[Then]
		public void Should_transition_to_the_failed_state()
		{
			_instance.CurrentState.ShouldEqual(_workflow.GetState(x => x.Failed));
		}

		[Then]
		public void Should_call_the_handler_method()
		{
			_handlerCalled.ShouldBeTrue();
		}

		[Then]
		public void Should_call_the_instance_handler_method()
		{
			_instanceHandlerCalled.ShouldBeTrue();
		}


		class Subject
		{
			public State CurrentState { get; set; }

			public void Create()
			{
				throw new SubjectException("EPIC FAIL!");
			}
		}


		class SubjectException :
			Exception
		{
			public SubjectException(string message)
				: base(message)
			{
			}
		}


		interface SubjectWorkflow
		{
			State Created { get; }
			State Failed { get; }

			Event Create { get; }
		}
	}

	[Scenario]
	public class When_throwing_an_exception_from_an_message_event_handler
	{
		bool _handlerCalled;
		WorkflowInstance<SubjectWorkflow> _instance;
		bool _instanceHandlerCalled;
		Subject _subject;
		StateMachineWorkflow<SubjectWorkflow, Subject> _workflow;
		bool _instanceMessageCalled;

		[When]
		public void Throwing_an_exception_from_an_event_handler()
		{
			_workflow = StateMachineWorkflow.New<SubjectWorkflow, Subject>(x =>
				{
					x.AccessCurrentState(y => y.CurrentState);

					x.Initially()
						.When(e => e.Create)
						.Then(i => i.Create)
						.TransitionTo(s => s.Created)
						.InCaseOf()
						.Exception<SubjectException>()
						.Then(() => _handlerCalled = true)
						.Then(i =>
							{
								_instanceHandlerCalled = true;
							})
						.Then((i, m) =>
							{
								_instanceMessageCalled = true;
							})
						.TransitionTo(s => s.Failed);
				});

			_subject = new Subject();

			_instance = _workflow.GetInstance(_subject);

			_instance.RaiseEvent(x => x.Create, new CreateArgs());
		}

		[Then]
		public void Should_transition_to_the_failed_state()
		{
			_instance.CurrentState.ShouldEqual(_workflow.GetState(x => x.Failed));
		}

		[Then]
		public void Should_call_the_handler_method()
		{
			_handlerCalled.ShouldBeTrue();
		}

		[Then]
		public void Should_call_the_instance_handler_method()
		{
			_instanceHandlerCalled.ShouldBeTrue();
		}

		[Then]
		public void Should_call_the_instance_message_method()
		{
			_instanceMessageCalled.ShouldBeTrue();
		}


		class Subject
		{
			public State CurrentState { get; set; }

			public void Create(CreateArgs args)
			{
				throw new SubjectException("EPIC FAIL!");
			}
		}


		class SubjectException :
			Exception
		{
			public SubjectException(string message)
				: base(message)
			{
			}
		}


		interface SubjectWorkflow
		{
			State Created { get; }
			State Failed { get; }

			Event<CreateArgs> Create { get; }
		}


		class CreateArgs
		{
		}
	}
}