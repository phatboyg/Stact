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
	using System;

	public interface Interceptor<T>
	{
		/// <summary>
		/// Called before a message is delivered to the output channel
		/// </summary>
		/// <param name="message">The message being delivered</param>
		/// <returns>The message passed, a modified message, or null to discard the message</returns>
		T OnSend(T message);

		/// <summary>
		/// Called when an exception is returned during delivery to the output channel
		/// </summary>
		/// <param name="exception">The exception that occurred</param>
		void OnException(Exception exception);

		/// <summary>
		/// Called when a message has been successfully delivered to the output channel
		/// </summary>
		void OnComplete();
	}
}