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
	/// Used to obtain and instance of a class based on the information contained
	/// in the specified message
	/// </summary>
	/// <typeparam name="TInstance">The type of the class</typeparam>
	/// <typeparam name="TChannel">The type of the message</typeparam>
	public interface InstanceProvider<TInstance, TChannel>
	{
		TInstance GetInstance(TChannel message);
	}
}