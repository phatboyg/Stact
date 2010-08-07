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
namespace Magnum.Infrastructure.Specs.Channels
{
	using System;
	using System.Linq;
	using Magnum.Channels;
	using Magnum.Concurrency;
	using Magnum.Extensions;
	using Magnum.Logging;
	using Magnum.Specs.StateMachine;
	using Magnum.TestFramework;
	using NHibernate;
	using NUnit.Framework;


	[Scenario]
	public class When_sending_a_message_to_an_nhibernate_backed_state_machine :
		Given_an_nhibernate_session_factory
	{
		decimal _newValue;

		[When]
		public void Sending_a_message_to_an_nhibernate_backed_state_machine()
		{
			TraceLogProvider.Configure(LogLevel.Debug);

			_newValue = new Random().Next(1, 500000)/100m;

			using (ISession session = SessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.CreateQuery("Delete TestStateMachineInstance").ExecuteUpdate();

				transaction.Commit();
			}

			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumersFor<TestStateMachineInstance>()
						.BindUsing<TestStateMachineInstanceBinding, int>()
						.ExecuteOnProducerThread()
						.CreateNewInstanceUsing(id => new TestStateMachineInstance(id))
						.PersistUsingNHibernate()
						.UseSessionProvider(() => SessionFactory.OpenSession());
				}))
			{
				input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
					{
						typeof(ChannelAdapter),
						typeof(BroadcastChannel),
						typeof(TypedChannelAdapter<CompleteOrder>),
						typeof(InstanceChannel<CompleteOrder>),
						typeof(TypedChannelAdapter<CreateOrder>),
						typeof(InstanceChannel<CreateOrder>),
						typeof(TypedChannelAdapter<UpdateOrder>),
						typeof(InstanceChannel<UpdateOrder>),
					});

				var future = new Future<int>();
				TestStateMachineInstance.CompletedLatch = new CountdownLatch(1, future.Complete);
				//
				input.Send(new CreateOrder
					{
						Id = 27
					});

				input.Send(new UpdateOrder
					{
						Id = 27,
						Value = _newValue,
					});

				input.Send(new CompleteOrder
					{
						Id = 27,
					});

				future.WaitUntilCompleted(5.Seconds()).ShouldBeTrue();
			}
		}

		[Then]
		[Category("Database")]
		public void Should_load_the_matching_instance_and_send_it_the_message()
		{
			using (ISession session = SessionFactory.OpenSession())
				session.Load<TestStateMachineInstance>(27).ShouldNotBeNull();
		}

		[Then]
		[Category("Database")]
		public void Should_have_updated_the_value_on_the_instance()
		{
			using (ISession session = SessionFactory.OpenSession())
			{
				var instance = session.Load<TestStateMachineInstance>(27);
				instance.ShouldNotBeNull();
				instance.Value.ShouldEqual(_newValue);
			}
		}
	}
}