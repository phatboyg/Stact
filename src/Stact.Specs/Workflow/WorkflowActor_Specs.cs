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
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Stact.Workflow;


	[Scenario]
	public class WorkflowActor_Specs
	{
		[Then]
		public void Easy_syntax_love()
		{
			ServiceController controller = null;

			ActorFactory<ServiceController> factory =
				StateMachineActorFactory.Create<ServiceWorkflow, ServiceController>(inbox =>
					{
						controller = new ServiceController(inbox);
						return controller;
					}, x =>
						{
							x.AccessCurrentState(s => s.CurrentState);

							x.Initially()
								.When(e => e.Started)
								.Then(i => i.OnStart())
								.TransitionTo(s => s.Running);

							x.During(s => s.Running)
								.When(e => e.Stopped)
								.Then(i => i.OnStop())
								.Finalize()
								.When(e => e.Paused)
								.Then(i => i.OnPause)
								.TransitionTo(s => s.Idle);

							x.During(s => s.Idle)
								.When(e => e.Started)
								.TransitionTo(s => s.Running);
						});

			ActorInstance service = factory.GetActor();

			service.Send(new Pause());
			service.Send(new Start());
			service.Send(new Start());
			service.Send(new Stop());

			try
			{
				controller.Started.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Never received start");
				controller.Paused.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Never received pause");
				controller.Stopped.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("Never received stop");
			}
			finally
			{
				service.Exit();
			}
		}


		class Pause
		{
		}


		class ServiceController :
			Actor
		{
			public ServiceController(Inbox inbox)
			{
				Paused = new Future<bool>();
				Started = new Future<bool>();
				Stopped = new Future<bool>();
			}

			public Future<bool> Started { get; private set; }
			public Future<bool> Stopped { get; private set; }
			public Future<bool> Paused { get; private set; }

			public State CurrentState { get; private set; }

			public void OnStart()
			{
				Started.Complete(true);
			}

			public void OnStop()
			{
				Stopped.Complete(true);
			}

			public void OnPause()
			{
				Paused.Complete(true);
			}
		}


		interface ServiceWorkflow
		{
			State Running { get; }
			State Idle { get; }

			Event<Start> Started { get; }

			Event<Pause> Paused { get; }
			Event<Stop> Stopped { get; }
		}


		class Start
		{
		}


		class Stop
		{
		}
	}
}