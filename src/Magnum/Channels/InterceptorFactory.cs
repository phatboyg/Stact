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
namespace Magnum.Channels
{
	/// <summary>
	/// Provides an instance of an interceptor for an InterceptorChannel to
	/// handle the interception of messages on channels
	/// </summary>
	/// <typeparam name="T">The message type</typeparam>
	public interface InterceptorFactory<T>
	{
		/// <summary>
		/// Returns an instance of an interceptor
		/// </summary>
		/// <param name="message">The message being delivered</param>
		/// <returns>An interceptor</returns>
		Interceptor<T> GetInterceptor(T message);
	}
}