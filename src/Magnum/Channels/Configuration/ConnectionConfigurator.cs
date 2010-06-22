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
namespace Magnum.Channels.Configuration
{
	/// <summary>
	/// A fluent syntax for defining subscriptions against a channel network
	/// </summary>
	public interface ConnectionConfigurator
	{
		/// <summary>
		/// Adds a channel directly without any modification
		/// </summary>
		/// <typeparam name="TChannel">The channel type</typeparam>
		/// <param name="channel">The channel instance</param>
		void AddChannel<TChannel>(Channel<TChannel> channel);

		/// <summary>
		/// Adds a channel directly without any modification
		/// </summary>
		/// <param name="channel">The channel instance</param>
		void AddUntypedChannel(UntypedChannel channel);

		/// <summary>
		/// Configures a new consumer
		/// </summary>
		/// <typeparam name="TChannel">The channel type</typeparam>
		/// <returns>A chainable method to configure additional options</returns>
		ChannelConnectionConfigurator<TChannel> AddConsumerOf<TChannel>();
	}
}