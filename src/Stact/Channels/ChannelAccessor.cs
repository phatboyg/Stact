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
	/// Provides a channel from an instance
	/// </summary>
	/// <typeparam name="TInstance">The type of instance being accessed</typeparam>
	/// <typeparam name="T">The type of channel to return</typeparam>
	/// <param name="instance">The instance to retrieve the channel from</param>
	/// <returns>The channel of the requested type from the instance</returns>
	public delegate Channel<T> ChannelAccessor<TInstance, T>(TInstance instance)
		where TInstance : class;
}