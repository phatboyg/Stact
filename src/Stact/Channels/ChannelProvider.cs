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
namespace Stact.Channels
{
	/// <summary>
	/// Used by an untyped channel dispatcher to get a channel where a message
	/// can be sent
	/// </summary>
	public interface ChannelProvider
	{
		Channel<T> GetChannel<T>(T message);
	}


	/// <summary>
	/// Used by dispatching channels to retrieve the appropriate channel for
	/// a message.
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public interface ChannelProvider<T>
	{
		/// <summary>
		/// Get a channel for the message
		/// </summary>
		/// <param name="message">The message to use to select the channel</param>
		/// <returns>The channel that should process the message, or null</returns>
		Channel<T> GetChannel(T message);
	}
}