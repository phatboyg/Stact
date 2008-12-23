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
namespace Magnum.Common.Specs.StateMachine
{
	using System.Diagnostics;
	using Common.StateMachine;

	public class ExampleStateMachine :
		StateMachine<ExampleStateMachine>
	{
		static ExampleStateMachine()
		{
			Define(() =>
				{
					SetInitialState(Idle);

					When(Idle,
					     OnEvent(Idle.Enter, x => x.EnteredIdleState()),
						 OnEvent(Idle.Leave, x => x.LeaveIdleState()));

					When(Idle,
					     OnEvent(CustomerEntered, (x,y) => x.InitiatedByEvent(y)),
					     OnEvent(CustomerCancelled, x => x.CancelTransaction()));

					When(TakingOrder,
						OnEvent(CustomerCancelled, x => x.CancelTransaction()));
				});
		}


		public static State Idle { get; set; }
		public static State TakingOrder { get; set; }

		public static Event CustomerEntered { get; set; }
		public static Event CustomerCancelled { get; set; }

		public void EnteredIdleState()
		{
			Trace.WriteLine("Entered idle state " + Current);
		}

		public void InitiatedByEvent(Event eevent)
		{
			Trace.WriteLine("Initiated by event " + eevent);

			TransitionTo(TakingOrder);
		}

		private void CancelTransaction()
		{
			Trace.WriteLine("Transaction cancelled while " + Current);
		}

		public void LeaveIdleState()
		{
			Trace.WriteLine("Left idle state " + Current);
		}

		public void Consume(ExampleOrder order)
		{
			RaiseEvent(CustomerEntered, order);
		}
	}
}