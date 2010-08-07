namespace Magnum.Infrastructure.Specs.Channels
{
	using System;
	using System.Linq;
	using Collections;
	using Concurrency;
	using Extensions;
	using Logging;
	using Magnum.Channels;
	using Magnum.Specs.StateMachine;
	using NHibernate;
	using NUnit.Framework;
	using TestFramework;


	[Scenario]
	public class When_sending_a_message_to_an_in_memory_state_machine_provider 
	{
		decimal _newValue;

		Cache<int, TestStateMachineInstance> _cache =
			new Cache<int, TestStateMachineInstance>(key => new TestStateMachineInstance(key));

		[When]
		public void Sending_a_message_to_an_nhibernate_backed_state_machine()
		{
			TraceLogProvider.Configure(LogLevel.Debug);

			_newValue = new Random().Next(1, 500000)/100m;

			var input = new ChannelAdapter();
			using (input.Connect(x =>
				{
					x.AddConsumersFor<TestStateMachineInstance>()
						.BindUsing<TestStateMachineInstanceBinding, int>()
						.ExecuteOnProducerThread()
						.CreateNewInstanceUsing(id => new TestStateMachineInstance(id))
						.CacheUsing(_cache);
				}))
			{
				AssertionsForCollections.ShouldEqual(input.Flatten().Select(c => c.GetType()), new[]
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

				AssertionsForBoolean.ShouldBeTrue(future.WaitUntilCompleted(5.Seconds()));
			}
		}

		[Then]
		public void Should_load_the_matching_instance_and_send_it_the_message()
		{
			_cache.Has(27).ShouldBeTrue();
		}

		[Then]
		public void Should_have_updated_the_value_on_the_instance()
		{
			_cache[27].Value.ShouldEqual(_newValue);
		}
	}
}