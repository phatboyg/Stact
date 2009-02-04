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
namespace Magnum.MapReduce
{
	using System;
	using System.Collections.Generic;

	public interface IRecordReader<K, V> :
		IDisposable
	{
		/// <summary>
		/// The position of the reader
		/// </summary>
		long Position { get; }

		/// <summary>
		/// The progress through the specified InputSplit
		/// </summary>
		float Progress { get; }

		/// <summary>
		/// Close the InputSplit
		/// </summary>
		void Close();

		/// <summary>
		/// Return the next record from the input file
		/// </summary>
		/// <returns></returns>
		KeyValuePair<K, V> Next();
	}
}