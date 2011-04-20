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
namespace Stact.Specs
{
	using Magnum.Extensions;
	using Magnum.TestFramework;
	using Routing;
	using Routing.Visualizers;


	[Scenario]
	public class When_routing_messages_using_the_routing_engine
	{
		[Then]
		public void Should_properly_invoke_the_message_receiver()
		{
			RoutingEngine engine = new DynamicRoutingEngine(new SynchronousFiber());

			var receivedA = new Future<A>();
			var receivedB = new Future<B>();

			engine.Configure(x =>
			{
				x.Receive<A>(receivedA.Complete);
				x.Receive<B>(receivedB.Complete);
			});

			engine.Send(new B());

			engine.Configure(x =>
			{
				new RoutingEngineTextVisualizer().Visit(x.Engine);
			});

			receivedB.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("B not received");
			receivedA.WaitUntilCompleted(2.Seconds()).ShouldBeTrue("A not received");
		}


		class A
		{
		}


		class B :
			A
		{
		}


		class C
		{
		}
	}
}