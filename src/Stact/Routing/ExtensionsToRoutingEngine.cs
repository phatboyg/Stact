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
namespace Stact.Routing
{
	using Internal;
	using Magnum.Extensions;
	using Stact.Internal;


	public static class ExtensionsToRoutingEngine
	{
		public static void Receive<T>(this RoutingEngine engine, Consumer<T> consumer)
		{
			var result = new Future<PendingReceive>();

			engine.Add(() =>
				{
					ConsumerNode<T> consumerNode = null;

					var locator = new JoinNodeLocator<T>(joinNode =>
						{
							consumerNode = new ConsumerNode<T>(new PoolFiber(), m =>
								{
									//joinNode.RemoveActivation(consumerNode);
									consumer(m);
								});

							joinNode.AddActivation(consumerNode);
						});

					locator.Search(engine);

					result.Complete(null);
				});

			result.WaitUntilCompleted(30.Seconds());
		}

		public static void Receive<T1, T2>(this RoutingEngine engine, Consumer<Tuple<T1,T2>> consumer)
		{
			engine.Add(() =>
				{
					ConsumerNode<Tuple<T1,T2>> consumerNode = null;

					var locator = new JoinNodeLocator<T1,T2>(joinNode =>
						{
							consumerNode = new ConsumerNode<Tuple<T1,T2>>(new PoolFiber(), m =>
								{
									//joinNode.RemoveActivation(consumerNode);
									consumer(m);
								});

							joinNode.AddActivation(consumerNode);
						});

					locator.Search(engine);
				});
		}
	}
}