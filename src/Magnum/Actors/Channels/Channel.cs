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
namespace Magnum.Actors.Channels
{
	using System;

	public delegate void Unsubscribe();

	public interface Channel
	{
		/// <summary>
		/// Remove all subscriptions to the channel
		/// </summary>
		void UnsubscribeAll();
	}

	public interface Channel<T> :
		Channel
	{
		/// <summary>
		/// Publish a message to the channel
		/// </summary>
		/// <param name="message"></param>
		/// <returns>True if there was a consumer for the message, otherwise false</returns>
		bool Publish(T message);

		/// <summary>
		/// Subscribes a consumer to the channel that will be called for every message that gets published
		/// </summary>
		/// <returns></returns>
		Unsubscribe Subscribe(Action<T> consumer);
	}
}