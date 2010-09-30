namespace Stact.Specs.StateMachine
{
	using System;
	using System.Runtime.Serialization;
	using Stact.StateMachine;

	[Serializable]
	public class OrderStateMachine :
		StateMachine<OrderStateMachine>
	{
		static OrderStateMachine()
		{
			Define(() =>
				{
					Initially(
						When(OrderSubmitted)
							.Then(workflow =>
								{
									// send request for payment
									// send barista the cup/order
								})
							.TransitionTo(WaitingForPayment),
						When(OrderCanceled)
							.Then(machine =>
								{
									// nothing has happened yet so we just complete 
								})
							.Complete());

					During(WaitingForPayment,
						When(PaymentSubmitted)
							.Call((machine) => machine.GetPaymentApproval())
							.TransitionTo(WaitingForPaymentApproval),
						When(OrderCanceled)
							.Then(machine =>
								{
									// notify barista that order was cancelled
								})
							.Then(machine =>
								{
									// act like we want to have another Then in the chain
								})
							.Complete());

					During(WaitingForPaymentApproval,
						When(PaymentApproved)
							.Then(machine =>
								{
									// since this machine only deals with the cashier, it is over
								}).Complete(),
						When(PaymentDenied)
							.Then(machine =>
								{
									// request payment again
								}).TransitionTo(WaitingForPayment));

					During(Completed,
						When(Completed.Enter)
							.Then(machine =>
								{
									// complete the transaction if required
								}));
				});
		}

		public OrderStateMachine()
		{
		}

		public OrderStateMachine(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

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

		public Guid TransactionId { get; set; }

		public void GetPaymentApproval()
		{
		}

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
	}
}