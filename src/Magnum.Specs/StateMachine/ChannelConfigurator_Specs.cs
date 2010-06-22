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
	using System.Linq;
	using Fibers;
	using Magnum.Channels;
	using Magnum.Extensions;
	using Magnum.StateMachine;
	using TestFramework;


	[Scenario]
	public class Connecting_a_state_machine_class_to_an_untyped_channel
	{
		ChannelConnection _disconnect;
		ChannelAdapter _input;
		SampleStateMachine _instance;

		[When]
		public void A_state_machine_is_connected_to_an_untyped_channel()
		{
			_instance = new SampleStateMachine();

			_input = new ChannelAdapter();
			_disconnect = _input.Connect(x =>
				{
					// adds a single state machine instance to the channel via listeners
					x.AddStateMachineInstance(new SynchronousFiber(), _instance);
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
					typeof(ChannelAdapter),
					typeof(BroadcastChannel),
					typeof(TypedChannelAdapter<SampleStarted>),
					typeof(ConsumerChannel<SampleStarted>),
					typeof(TypedChannelAdapter<SampleStopped>),
					typeof(ConsumerChannel<SampleStopped>),
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