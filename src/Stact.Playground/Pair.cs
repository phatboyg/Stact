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
	public static class Pair
	{
		public static Pair<T1, T2> New<T1, T2>(T1 first, T2 second)
		{
			return new Pair<T1, T2>(first, second);
		}
	}


	public class Pair<T1, T2>
	{
		public readonly T1 First;
		public readonly T2 Second;

		public Pair(T1 first, T2 second)
		{
			First = first;
			Second = second;
		}
	}
}