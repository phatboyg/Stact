namespace Stact.Specs.StateMachine
{
	using System.Linq;
	using Magnum.Extensions;
	using Magnum.StateMachine;
	using Magnum.TestFramework;
	using Stact.Channels;

	[Scenario]
	public class Connecting_a_state_machine_class_to_an_untyped_channel
	{
		private ChannelConnection _disconnect;
		private ChannelAdapter _input;
		private SampleStateMachine _instance;

		[When]
		public void A_state_machine_is_connected_to_an_untyped_channel()
		{
			_instance = new SampleStateMachine();

			_input = new ChannelAdapter();
			_disconnect = _input.Connect(x =>
				{
					// adds a single state machine instance to the channel via listeners
					x.AddConsumersFor<SampleStateMachine>()
						.UsingInstance(_instance)
						.HandleOnCallingThread();
				});

			_input.Send(new SampleStarted());
			_input.Send(new SampleStopped());
		}

		[After]
		public void After()
		{
			_disconnect.Dispose();
		}

		[Then]
		public void Should_have_a_set_of_channel_adapters_for_each_message_type()

		{
			_input.Flatten().Select(c => c.GetType()).ShouldEqual(new[]
				{
					typeof (ChannelAdapter),
					typeof (BroadcastChannel),
					typeof (TypedChannelAdapter<SampleStarted>),
					typeof (ConsumerChannel<SampleStarted>),
					typeof (TypedChannelAdapter<SampleStopped>),
					typeof (ConsumerChannel<SampleStopped>),
				});
		}

		[Then]
		public void Should_transition_to_the_running_state()
		{
			_instance.WasStarted.WaitUntilCompleted(1.Seconds()).ShouldBeTrue();
		}

		[Then]
		public void Should_transition_to_the_completed_state()
		{
			_instance.WasStopped.WaitUntilCompleted(1.Seconds()).ShouldBeTrue();
		}


		public class SampleStarted
		{
		}


		public class SampleStateMachine :
			StateMachine<SampleStateMachine>
		{
			static SampleStateMachine()
			{
				Define(() =>
					{
						Initially(
							When(Started)
								.Call((s, m) => s.WasStarted.Complete(m))
								.TransitionTo(Running));

						During(Running,
						       When(Stopped)
						       	.Call((s, m) => s.WasStopped.Complete(m))
						       	.TransitionTo(Completed));
					});
			}

			public SampleStateMachine()
			{
				WasStarted = new Future<SampleStarted>();
				WasStopped = new Future<SampleStopped>();
			}

			public static State Initial { get; set; }
			public static State Completed { get; set; }
			public static State Running { get; set; }


			public static Event<SampleStarted> Started { get; set; }
			public static Event<SampleStopped> Stopped { get; set; }

			public Future<SampleStarted> WasStarted { get; private set; }
			public Future<SampleStopped> WasStopped { get; private set; }
		}


		public class SampleStopped
		{
		}
	}
}