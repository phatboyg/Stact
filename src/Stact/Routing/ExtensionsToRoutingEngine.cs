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
	using System;
	using Configuration;
	using Internal;


	public static class ExtensionsToRoutingEngine
	{
		public static void Receive<T>(this RoutingEngineConfigurator configurator, Consumer<T> consumer)
		{
			var consumerNode = new ConsumerNode<T>(consumer);

			configurator.Add(consumerNode);
		}

		public static void Receive<T>(this RoutingEngineConfigurator configurator, Fiber fiber, Consumer<T> consumer)
		{
			var consumerNode = new ConsumerNode<T>(fiber, consumer);

			configurator.Add(consumerNode);
		}

		public static void Receive<T1, T2>(this RoutingEngineConfigurator configurator, Consumer<Tuple<T1, T2>> consumer)
		{
			var consumerNode = new ConsumerNode<Tuple<T1,T2>>(consumer);

			configurator.Add(consumerNode);
		}

		public static void Receive<T1, T2>(this RoutingEngineConfigurator configurator, Fiber fiber, Consumer<Tuple<T1, T2>> consumer)
		{
			var consumerNode = new ConsumerNode<Tuple<T1,T2>>(fiber, consumer);

			configurator.Add(consumerNode);
		}
	}
}