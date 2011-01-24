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


	public static class Semigroup
	{
		public static Semigroup<bool> BooleanAnd = new Semigroup<bool>(x => y => x && y);
		public static Semigroup<bool> BooleanOr = new Semigroup<bool>(x => y => x || y);
		public static Semigroup<bool> BooleanXor = new Semigroup<bool>(x => y => (x && !y) || (!x && y));
		public static Semigroup<decimal> DecimalAdd = new Semigroup<decimal>(x => y => x + y);
		public static Semigroup<decimal> DecimalMax = new Semigroup<decimal>(Ordering.DecimalOrder.Max);
		public static Semigroup<decimal> DecimalMin = new Semigroup<decimal>(Ordering.DecimalOrder.Min);
		public static Semigroup<decimal> DecimalMultiply = new Semigroup<decimal>(x => y => x*y);
		public static Semigroup<int> IntNext = new Semigroup<int>(Ordering.IntOrder.Next);
		public static Semigroup<int> IntAdd = new Semigroup<int>(x => y => x + y);
		public static Semigroup<int> IntMax = new Semigroup<int>(Ordering.IntOrder.Max);
		public static Semigroup<int> IntMin = new Semigroup<int>(Ordering.IntOrder.Min);
		public static Semigroup<int> IntMultiply = new Semigroup<int>(x => y => x*y);
		public static Semigroup<long> LongAdd = new Semigroup<long>(x => y => x + y);
		public static Semigroup<long> LongMax = new Semigroup<long>(Ordering.LongOrder.Max);
		public static Semigroup<long> LongMin = new Semigroup<long>(Ordering.LongOrder.Min);
		public static Semigroup<long> LongMultiply = new Semigroup<long>(x => y => x*y);

		public static Semigroup<string> StringAppend = new Semigroup<string>(x => y => string.Concat(x, y));
	}


	public class Semigroup<M>
	{
		readonly Func<M, Func<M, M>> _sum;

		public Semigroup(Func<M, Func<M, M>> sum)
		{
			_sum = sum;
		}

		public Semigroup(Func<M, M, M> sum)
		{
			_sum = x => y => sum(x, y);
		}

		public M Sum(M left, M right)
		{
			return _sum(left)(right);
		}

		public Func<M, M> Sum(M value)
		{
			return _sum(value);
		}

		public Func<M, Func<M, M>> Sum()
		{
			return _sum;
		}

		public Semigroup<Func<N, M>> Map<N>()
		{
			Func<Func<N, M>, Func<N, M>, Func<N, M>> mapf = (x, y) =>
				{
					return n => _sum(x(n))(y(n));
				};

			return new Semigroup<Func<N, M>>(mapf);
		}
	}
}