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
	using Magnum.TestFramework;
	using NUnit.Framework;
	using Routing;
	using Routing.Visualizers;
	using Visualizers;


	[Scenario]
	public class When_routing_messages_using_the_routing_engine
	{
		Future<A> _receivedA;
		Future<B> _receivedB;
		Future<C> _receivedC;
		RoutingEngine _engine;

		[When]
		public void Should_properly_invoke_the_message_receiver()
		{
			_engine = new DynamicRoutingEngine(new SynchronousFiber());

			_receivedA = new Future<A>();
			_receivedB = new Future<B>();
			_receivedC = new Future<C>();

			_engine.Configure(x =>
			{
				x.Receive<A>(_receivedA.Complete);
				x.Receive<B>(_receivedB.Complete);
				x.Receive<C>(_receivedC.Complete);
			});
		}

		[Then, Explicit]
		public void Display_graph()
		{
			RoutingEngineDebugVisualizer.Show(_engine);
		}

		[Then]
		public void Should_not_receive_an_a()
		{
			_engine.Send(new B());
			_engine.Send(new C());

			_receivedA.IsCompleted.ShouldBeFalse();
		}

		[Then]
		public void Should_receive_a_b()
		{
			_engine.Send(new B());
			_engine.Send(new C());

			_receivedB.IsCompleted.ShouldBeTrue();
		}

		[Then]
		public void Should_receive_a_c()
		{
			_engine.Send(new B());
			_engine.Send(new C());

			_receivedC.IsCompleted.ShouldBeTrue();
		}


		class A
		{
		}


		class B :
			A
		{
		}


		class C :
			B
		{
		}
	}
}