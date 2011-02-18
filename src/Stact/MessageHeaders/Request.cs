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
	/// <summary>
	/// Request is a message stereotype that is applied to a message in which
	/// a response is expected
	/// </summary>
	/// <typeparam name = "T">The message type</typeparam>
	public interface Request<out T> :
		Message<T>,
		RequestHeader
	{
		/// <summary>
		///   Where responses to the request should be sent
		/// </summary>
		UntypedChannel ResponseChannel { get; }
	}
}