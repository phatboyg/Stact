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
	using System;


	public static class Ordering
	{
		public static Ordering<decimal> DecimalOrder = new Ordering<decimal>();
		public static Ordering<int> IntOrder = new Ordering<int>();
		public static Ordering<long> LongOrder = new Ordering<long>();
	}


	public class Ordering<T>
		where T : IComparable<T>
	{
		public T Next(T left, T right)
		{
			return (right.CompareTo(default(T)) == 0) ? left : right;
		}

		public T Max(T left, T right)
		{
			return IsGreaterThan(left, right) ? left : right;
		}

		public T Min(T left, T right)
		{
			return IsLessThan(left, right) ? left : right;
		}

		public bool IsLessThan(T left, T right)
		{
			return left.CompareTo(right) < 0;
		}

		public bool IsLessThanOrEqual(T left, T right)
		{
			return left.CompareTo(right) <= 0;
		}

		public bool IsGreaterThan(T left, T right)
		{
			return left.CompareTo(right) > 0;
		}

		public bool IsGreaterThanOrEqual(T left, T right)
		{
			return left.CompareTo(right) >= 0;
		}
	}
}