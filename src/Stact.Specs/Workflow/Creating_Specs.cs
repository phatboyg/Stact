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
	using System.Diagnostics;
	using NUnit.Framework;
	using Stact.Workflow;


	[TestFixture]
	public class Creating_a_state_machine_workflow
	{
		[Test]
		public void Should_be_easy()
		{
			StateMachineWorkflow<RemoteRequestEngineWorkflow, RemoteRequestEngine> workflow =
				StateMachineWorkflow.New<RemoteRequestEngineWorkflow, RemoteRequestEngine>(x =>
					{
						x.AccessCurrentState(i => i.CurrentState);

						x.Initially()
							.When(w => w.Start)
							.TransitionTo(y => y.Running)
							.When(y => y.Stop)
							.TransitionTo(y => y.Stopped);

						x.During(y => y.Running)
							.When(y => y.Interrupted)
							.Then(i => i.CancelPendingRequest)
							.When(y => y.Stop)
							.Then(i => i.CancelAllPendingRequests)
							.TransitionTo(y => y.Stopped);

						x.During(y => y.Stopped)
							.When(y => y.Start)
							.TransitionTo(y => y.Running)
							.When(y => y.Dispose)
							.Finalize();

						x.Finally()
							.Then(instance => Trace.WriteLine("Completed"));
					});

			var visitor = new TraceStateMachineVisitor();
			workflow.Accept(visitor);

			var engine = new RemoteRequestEngine();
			WorkflowInstance<RemoteRequestEngineWorkflow> engineInstance = workflow.GetInstance(engine);

			engineInstance.RaiseEvent(x => x.Start);
			engineInstance.RaiseEvent(x => x.Interrupted, new InterruptImpl {Source = "End of the world" });
			engineInstance.RaiseEvent(x => x.Stop);
			engineInstance.RaiseEvent(x => x.Dispose);

			Trace.WriteLine("Final State: " + engineInstance.CurrentState);
		}


		class RemoteRequestEngine
		{
			public State CurrentState { get; set; }

			public void CancelPendingRequest(Interrupt message)
			{
				Trace.WriteLine("Cancelling request: " + message.Source);
			}

			public void CancelAllPendingRequests()
			{
				Trace.WriteLine("Canceling all pending requests");
			}
		}


		interface Interrupt
		{
			string Source { get; }
		}

		class InterruptImpl :
			Interrupt
		{
			public string Source { get; set; }
		}

		/// <summary>
		/// The interface defines the events and states supported by the workflow and is 
		/// used for strongly-typed binding to avoid magic strings used as event/state names
		/// </summary>
		interface RemoteRequestEngineWorkflow
		{
			// these are simple events with no body content
			Event Start { get; }
			Event Stop { get; }
			Event Dispose { get; }

			// this is a message event with body content specified by a type
			Event<Interrupt> Interrupted { get; }

			// these are states supported by the workflow
			State Running { get; }
			State Stopped { get; }
		}
	}


	[TestFixture]
	public class Creating_a_state_machine_workflow_with_heavy_nested_closure
	{
		[Test]
		public void Should_be_easy()
		{
			StateMachineWorkflow<RemoteRequestEngineWorkflow, RemoteRequestEngine> workflow =
				StateMachineWorkflow.New<RemoteRequestEngineWorkflow, RemoteRequestEngine>(x =>
					{
						x.AccessCurrentState(i => i.CurrentState);

						x.Initially(c =>
							{
								c.When(e => e.Start, t =>
									{
										t.TransitionTo(y => y.Running);
									});

								c.When(e => e.Stop, t =>
									{
										t.TransitionTo(y => y.Stopped);
									});
							});

						x.During(s => s.Running, c =>
							{
								c.When(e => e.Interrupted, t =>
									{
										t.TransitionTo(y => y.Stopped);
									});

								c.When(e => e.Stop, t =>
									{
										t.TransitionTo(y => y.Stopped);
									});
							});

						x.During(s => s.Stopped, c =>
							{
								c.When(e => e.Start, t =>
									{
										t.TransitionTo(y => y.Running);
									});

								c.When(e => e.Dispose, t =>
									{
										t.Finalize();
									});
							});

						x.Finally(t =>
							{
								t.Then(instance => Trace.WriteLine("Completed"));
							});
					});

			var visitor = new TraceStateMachineVisitor();
			workflow.Accept(visitor);

			var engine = new RemoteRequestEngine();
			WorkflowInstance<RemoteRequestEngineWorkflow> engineInstance = workflow.GetInstance(engine);

			engineInstance.RaiseEvent(x => x.Start);
			engineInstance.RaiseEvent(x => x.Stop);
			engineInstance.RaiseEvent(x => x.Dispose);

			Trace.WriteLine("Final State: " + engineInstance.CurrentState);
		}


		class RemoteRequestEngine
		{
			public State CurrentState { get; set; }
		}


		interface Interrupt
		{
			string Source { get; }
		}


		/// <summary>
		/// The interface defines the events and states supported by the workflow and is 
		/// used for strongly-typed binding to avoid magic strings used as event/state names
		/// </summary>
		interface RemoteRequestEngineWorkflow
		{
			// these are simple events with no body content
			Event Start { get; }
			Event Stop { get; }
			Event Dispose { get; }

			// this is a message event with body content specified by a type
			Event<Interrupt> Interrupted { get; }

			// these are states supported by the workflow
			State Running { get; }
			State Stopped { get; }
		}
	}
}