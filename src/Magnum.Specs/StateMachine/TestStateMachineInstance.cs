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
namespace Magnum.Specs.StateMachine
{
	using System;
	using Magnum.Concurrency;
	using Magnum.StateMachine;


	public class TestStateMachineInstance :
		StateMachine<TestStateMachineInstance>
	{
		static TestStateMachineInstance()
		{
			Define(() =>
				{
					Initially(
					          When(CreatingOrder)
					          	.Then(s =>
					          		{
					          			s.Value = 0.0m;
					          			s.CreatedAt = SystemUtil.UtcNow;
					          		})
					          	.TransitionTo(Created));

					During(Created,
					       When(UpdatingOrder)
					       	.Call((s, m) => s.UpdateOrder(m))
					       	.TransitionTo(Updated));

					During(Updated,
					       When(UpdatingOrder)
					       	.Call((s, m) => s.UpdateOrder(m)),
					       When(CompletingOrder)
					       	.Then(s =>
					       		{
					       			s.CompletedAt = SystemUtil.UtcNow;
					       			if (CompletedLatch != null)
					       				CompletedLatch.CountDown();
					       		})
					       	.TransitionTo(Completed));
				});
		}

		public TestStateMachineInstance(int id)
		{
			Id = id;
		}

		protected TestStateMachineInstance()
		{
		}

		public virtual int Id { get; private set; }
		public virtual decimal Value { get; private set; }
		public virtual DateTime? CreatedAt { get; private set; }
		public virtual DateTime? UpdatedAt { get; private set; }
		public virtual DateTime? CompletedAt { get; private set; }
		public virtual DateTime? LastUpdatedAt { get; private set; }

		public static State Initial { get; set; }
		public static State Created { get; set; }
		public static State Updated { get; set; }
		public static State Completed { get; set; }

		public static Event<CreateOrder> CreatingOrder { get; set; }
		public static Event<UpdateOrder> UpdatingOrder { get; set; }
		public static Event<CompleteOrder> CompletingOrder { get; set; }

		public static CountdownLatch CompletedLatch { get; set; }

		void UpdateOrder(UpdateOrder message)
		{
			Value += message.Value;
			UpdatedAt = SystemUtil.UtcNow;
		}
	}


	public abstract class OrderMessage
	{
		public int Id { get; set; }
	}


	public class CreateOrder :
		OrderMessage
	{
	}


	public class UpdateOrder :
		OrderMessage
	{
		public decimal Value { get; set; }
	}


	public class CompleteOrder :
		OrderMessage
	{
	}
}