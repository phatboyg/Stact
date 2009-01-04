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
	using System;
	using System.Runtime.Serialization;
	using Common.StateMachine;

	/// <summary>
	/// An example showing the the StateMachine can be used to define the states, events, and 
	/// conditions for state transitions (along with behavior when the state changes).
	/// 
	/// These must be marked as serializable and have the default and serialization constructor
	/// if you want to be able to persist the state of the machine using the binary formatter.
	/// </summary>
	[Serializable]
	public class ExampleStateMachine :
		StateMachine<ExampleStateMachine>
	{
		/// <summary>
		/// The static initializer is used to define the state machine. This is called once
		/// for the state machine (for performance reasons).
		/// </summary>
		static ExampleStateMachine()
		{
			Define(() =>
				{
					// eliminated, ceremony, just use the default states with the right names
					// SetInitialState(Initial);
					// SetCompletedState(Completed);

					Initially(
						When(CommentCardReceived, (workflow, @event, message) =>
							{
								if (message.IsComplaint)
									workflow.TransitionTo(WaitingForManager);
								else
									workflow.TransitionTo(Completed);
							}),
						When(OrderSubmitted, machine =>
							{
								// send request for payment
								// send barista the cup/order
								machine.TransitionTo(WaitingForPayment);
							}),
						When(OrderCanceled, machine =>
							{
								// nothing has happened yet so we just complete 
                                machine.Complete();
							}));

					During(WaitingForPayment,
						When(PaymentSubmitted, machine =>
							{
								// send payment data for approval
								machine.TransitionTo(WaitingForPaymentApproval);
							}),
						When(OrderCanceled, machine =>
							{
								// notify barista that order was cancelled
								machine.Complete();
							}));

					During(WaitingForPaymentApproval,
						When(PaymentApproved, machine =>
							{
								// since this machine only deals with the cashier, it is over
							    machine.Complete();
							}),
						When(PaymentDenied, machine =>
							{
								// request payment again
								machine.TransitionTo(WaitingForPayment);
							}));

					During(Completed,
						When(Completed.Enter, machine =>
							{
								// complete the transaction if required
							}));
				});
		}

		public ExampleStateMachine()
		{}

		public ExampleStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{}

		public static State Initial { get; set; }
		public static State WaitingForPayment { get; set; }
		public static State WaitingForPaymentApproval { get; set; }
		public static State Completed { get; set; }
		public static State WaitingForManager { get; set; }

		public static Event OrderSubmitted { get; set; }
		public static Event PaymentSubmitted { get; set; }
		public static Event PaymentApproved { get; set; }
		public static Event PaymentDenied { get; set; }
		public static Event OrderCanceled { get; set; }

		public static Event<CommentCard> CommentCardReceived { get; set; }

		public Guid TransactionId { get; set; }

		public void SubmitOrder()
		{
			RaiseEvent(OrderSubmitted);
		}

		public void SubmitPayment()
		{
			RaiseEvent(PaymentSubmitted);
		}

		public void ApprovePayment()
		{
			RaiseEvent(PaymentApproved);
		}

		public void SubmitCommentCard(CommentCard card)
		{
			RaiseEvent(CommentCardReceived, card);
		}
	}

	public class CommentCard
	{
		public bool IsComplaint { get; set; }
	}
}