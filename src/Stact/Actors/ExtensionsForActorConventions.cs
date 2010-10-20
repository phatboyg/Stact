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
namespace Stact
{
	using Configuration;
	using Internal;


	public static class ExtensionsForActorConventions
	{
		/// <summary>
		/// Connect methods that are public and have a single parameter that is a Message type, such as
		/// Message, Request, or Response as a consumer on the inbox
		/// </summary>
		/// <typeparam name="TActor"></typeparam>
		/// <param name="configurator"></param>
		public static void ConnectPublicMethods<TActor>(this ActorFactoryConfigurator<TActor> configurator)
			where TActor : Actor
		{
			var convention = new PublicMethodsConvention<TActor>();
			configurator.AddConvention(convention);
		}

		/// <summary>
		/// Connect properties that are public and of type Channel&lt;T&gt; to the inbox
		/// </summary>
		/// <typeparam name="TActor"></typeparam>
		/// <param name="configurator"></param>
		public static void ConnectPropertyChannels<TActor>(this ActorFactoryConfigurator<TActor> configurator)
			where TActor : Actor
		{
			var convention = new PropertyChannelsConvention<TActor>();
			configurator.AddConvention(convention);
		}
	}
}