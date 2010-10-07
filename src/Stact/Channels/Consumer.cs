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
	/// A consumer delegate, which can be assigned to any method that takes a message as an argument,
	/// including Actions, void methods, etc.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="message"></param>
	public delegate void Consumer<T>(T message);
}