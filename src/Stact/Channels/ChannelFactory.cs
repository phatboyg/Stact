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
namespace Stact.Channels
{
	/// <summary>
	/// Implemented by classes that can create channels. The factory method is used
	/// to create channels outside of any message delivery context. If the content
	/// of the message is important in the creation of the channel, the ChannelProvider is a 
	/// better choice to use since it passes the message as part of the channel acquisition
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ChannelFactory<T>
	{
		/// <summary>
		/// Returns the channel
		/// </summary>
		/// <returns>A channel instance</returns>
		Channel<T> GetChannel();
	}
}