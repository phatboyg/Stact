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
namespace Stact.Routing.Configuration
{
	using System;


	public static class WhenExtensions
	{
		public static RoutingConfigurator<T> When<T>(this RoutingEngineConfigurator configurator)
		{
			var whenConfigurator = new WhenConfiguratorImpl<T>();

			configurator.Add(whenConfigurator);

			return whenConfigurator;
		}

		public static WhenConfigurator<T> When<T>(this Inbox inbox)
		{
			throw new NotImplementedException();
		}

	}

	public static class ReceiveExtensions
	{
		public static ReceiveConfigurator<T> Receive<T>(this WhenConfigurator<T> configurator, SelectiveConsumer<T> consumer)
		{
			var recieveConfigurator = new ReceiveConfiguratorImpl<T>();


			return recieveConfigurator;

		}
	}
}

