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
namespace Magnum.RulesEngine
{
	using System;

	/// <summary>
	/// A session created to assert objects into the rules engine for evaluation. Once all the objects
	/// are asserted, the Run method should be called to run all the rules that apply to the objects that
	/// are present.
	/// </summary>
	public interface StatefulSession :
		IDisposable
	{
		/// <summary>
		/// Asserts an object into the session for evaluation by the rules
		/// </summary>
		/// <typeparam name="T">The implied type of the object being asserted</typeparam>
		/// <param name="obj">The object being asserted</param>
		void Assert<T>(T obj);

		/// <summary>
		/// Activates any rules that apply to the objects asserted into the session
		/// </summary>
		void Run();
	}
}