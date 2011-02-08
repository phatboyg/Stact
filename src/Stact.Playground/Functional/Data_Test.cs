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
namespace Stact.Functional
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using Data;


	public class Data_Test
	{
		void Test_Sequence()
		{
			var random = new Random();
			int[] values = Enumerable.Range(0, 1000)
				.Select(x => random.Next(0, 1000))
				.ToArray();

			Sequence<int> seq = Sequence<int>.Empty();
			for (int i = 0; i < values.Length; i++)
				seq = seq.PushBack(values[i]);

			Trace.WriteLine("Count: " + seq.Count);

			var sb = new StringBuilder(":> ");
		}
	}
}