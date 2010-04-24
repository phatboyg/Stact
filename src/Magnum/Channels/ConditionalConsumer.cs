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
	using System;

	/// <summary>
	///   A conditional consumer is given a message to evaluate, after which it
	///   can determine if it is interested in the message and return an action
	///   to process the message or null
	/// </summary>
	/// <typeparam name = "T">The message type</typeparam>
	/// <param name = "message">The message</param>
	/// <returns>An action to consume the message, or null</returns>
	public delegate Consumer<T> ConditionalConsumer<T>(T message);
}