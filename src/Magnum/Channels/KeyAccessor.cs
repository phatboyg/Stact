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
	/// Returns the key for the given source
	/// </summary>
	/// <typeparam name="T">The type of the source from which the key is to be retrieved</typeparam>
	/// <typeparam name="TKey">The type of the key to return</typeparam>
	/// <param name="source">The source from which to retrieve the key</param>
	/// <returns>The value of the key for the source</returns>
	public delegate TKey KeyAccessor<in T, out TKey>(T source);
}