// Copyright 2007-2010 The Apache Software Foundation.
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
	/// Defines the instance policy for a regarding a particular event
	/// </summary>
	/// <typeparam name="T">The state machine type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public interface InstanceChannelPolicy<T, TChannel>
		where T : class
	{
		/// <summary>
		/// Returns true if the message can create a new instance
		/// </summary>
		/// <param name="message">The message received</param>
		/// <param name="instance">The instance created</param>
		/// <returns>True if the new instance could be created</returns>
		bool CanCreateInstance(TChannel message, out T instance);

		/// <summary>
		/// Checks if the message can be handled by an existing instance
		/// </summary>
		/// <param name="message">The message received</param>
		/// <returns>True if the message should be delivered to the instance, otherwise false</returns>
		bool IsHandledByExistingInstance(TChannel message);

		/// <summary>
		/// Called when a message was not handled by an instance
		/// </summary>
		/// <param name="message">The message that was not handled</param>
		void WasNotHandled(TChannel message);

		/// <summary>
		/// Checks if an instance can be unloaded
		/// </summary>
		/// <param name="instance">The instance to check</param>
		/// <returns>True if the instance can be unloaded from memory</returns>
		bool CanUnloadInstance(T instance);

		/// <summary>
		/// Checks if an instance can be removed/destroyed
		/// </summary>
		/// <param name="instance">The instance to check</param>
		/// <returns>True if the instance can be removed from storage</returns>
		bool CanRemoveInstance(T instance);
	}
}