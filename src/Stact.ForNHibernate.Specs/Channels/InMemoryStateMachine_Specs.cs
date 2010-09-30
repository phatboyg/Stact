// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Stact.ForNHibernate.Specs.Channels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Collections;
	using Concurrency;
	using Extensions;
	using Logging;
	using Stact.Channels;
	using Stact.Specs.StateMachine;
	using TestFramework;


	[Scenario]
	public class When_sending_a_message_to_an_in_memory_state_machine_provider
	{
		Cache<int, TestStateMachineInstance> _cache =
			new Cache<int, TestStateMachineInstance>(key => new TestStateMachineInstance(key));

		IEnumerable<Type> _networkTypes;
		decimal _newValue;

		[When]
		public void Sending_a_message_to_an_nhibernate_backed_state_machine()
		{
			TraceLogger.Configure(LogLevel.Debug);

			_newValue = new Random().Next(1, 500000)/100m;

			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumersFor<TestStateMachineInstance>()
						.BindUsing<TestStateMachineInstanceBinding, int>()
						.HandleOnCallingThread()
						.CreateNewInstanceBy(id => new TestStateMachineInstance(id))
						.PersistInMemoryUsing(_cache);
				}))
			{
				_networkTypes = input.Flatten().Select(c => c.GetType());

				var future = new Future<int>();
				TestStateMachineInstance.CompletedLatch = new CountdownLatch(1, future.Complete);
				//
				input.Send(new UpdateOrder
					{
						Id = 47,
						Value = _newValue,
					});

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
		public void Should_have_the_proper_network_configuration()
		{
			_networkTypes.ShouldEqual(new[]
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
		}

		[Then]
		public void Should_load_the_matching_instance_and_send_it_the_message()
		{
			_cache.Has(27).ShouldBeTrue();
		}

		[Then]
		public void Should_not_have_the_missing_instance()
		{
			_cache.Has(47).ShouldBeFalse();
		}

		[Then]
		public void Should_have_updated_the_value_on_the_instance()
		{
			_cache[27].Value.ShouldEqual(_newValue);
		}
	}
}