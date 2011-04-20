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
	using Magnum.TestFramework;
	using NUnit.Framework;
	using Routing;
	using Routing.Internal;
	using Routing.Visualizers;


	[Scenario]
	public class When_adding_a_request_message
	{
		DynamicRoutingEngine _engine;
		Fiber _fiber;

		[When]
		public void Adding_a_request_message()
		{
			_fiber = new SynchronousFiber();
			_engine = new DynamicRoutingEngine(_fiber);

			_engine.Request(new A(), new ShuntChannel());
		}

		[Then]
		[Explicit]
		public void Show_me_the_graph()
		{
			var visualizer = new TraceRoutingEngineVisualizer();
			visualizer.Show(_engine);
		}

		[Then]
		public void Should_result_in_two_paths_through_the_routing_engine()
		{
			var foundA = new Future<AlphaNode<Message<A>>>();
			var foundRequestA = new Future<AlphaNode<Request<A>>>();

			new MatchAlphaNode<Message<A>>(_engine, foundA.Complete);
			new MatchAlphaNode<Request<A>>(_engine, foundRequestA.Complete);

			foundRequestA.IsCompleted.ShouldBeTrue("Could not find request alpha node");
			foundA.IsCompleted.ShouldBeTrue("Could not find body alpha node");

			var visualizer = new TraceRoutingEngineVisualizer();
			visualizer.Show(_engine);
		}


		class A
		{
		}
	}
}