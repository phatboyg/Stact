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
namespace Stact.Specs.Behavior
{
	using System;
	using Magnum.TestFramework;
	using NUnit.Framework;
	using Routing;
	using Visualizers;


    /* drop this features i think
	[Scenario]
	public class When_receiving_two_messages_at_once
	{
		DynamicRoutingEngine _engine;
		Future<Tuple<A, B>> _received;

		[When]
		public void Receiving_two_messages_at_once()
		{
			_received = new Future<Tuple<A, B>>();

			_engine = new DynamicRoutingEngine(new SynchronousFiber());
			_engine.Configure(x =>
			{
				x.Receive<A, B>(messages => _received.Complete(messages));
			});
		}

		[Then]
		[Explicit]
		public void Display_graph()
		{
			RoutingEngineDebugVisualizer.Show(_engine);
		}

		[Then]
		[Explicit]
		public void Display_graph_of_header_types()
		{
			var engine = new DynamicRoutingEngine(new SynchronousFiber());
			engine.Configure(x => {
				x.Receive<Message<A>, Message<B>>(messages =>
				{
				});
			});

			RoutingEngineDebugVisualizer.Show(engine);
		}

		[Then]
		public void Should_receive_the_message_tuple()
		{
			_engine.Send(new A());
			_engine.Send(new B());

			_received.IsCompleted.ShouldBeTrue();
		}


		class A
		{
		}


		class B
		{
		}
	}
     */
}