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
namespace Stact.Data.Internal
{
	public struct Monoid<T>
	{
		public delegate T MonoidOperation(T t1, T t2);


		public readonly MonoidOperation Operation;
		public readonly T Zero;

		public Monoid(T zero, MonoidOperation operation)
		{
			Zero = zero;
			Operation = operation;
		}
	}
}