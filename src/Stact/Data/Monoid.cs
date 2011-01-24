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
namespace Stact.Data
{
	using System;
	using Internal;


	public static class Monoid
	{
		public static Monoid<bool> BooleanAnd = new Monoid<bool>(Semigroup.BooleanAnd, true);
		public static Monoid<bool> BooleanOr = new Monoid<bool>(Semigroup.BooleanOr, false);
		public static Monoid<bool> BooleanXor = new Monoid<bool>(Semigroup.BooleanXor, false);
		public static Monoid<decimal> DecimalAdd = new Monoid<decimal>(Semigroup.DecimalAdd, 0.0m);
		public static Monoid<decimal> DecimalMultiply = new Monoid<decimal>(Semigroup.DecimalMultiply, 1.0m);
		public static Monoid<int> IntAdd = new Monoid<int>(Semigroup.IntAdd, 0);
		public static Monoid<int> IntMultiply = new Monoid<int>(Semigroup.IntMultiply, 1);
		public static Monoid<long> LongAdd = new Monoid<long>(Semigroup.LongAdd, 0);
		public static Monoid<long> LongMultiply = new Monoid<long>(Semigroup.LongMultiply, 1);
		public static Monoid<string> StringAppend = new Monoid<string>(Semigroup.StringAppend, string.Empty);
	}


	public class Monoid<M>
	{
		public readonly M Identity;
		readonly Func<M, Func<M, M>> _append;

		protected Monoid(Func<M, Func<M, M>> append, M identity)
		{
			_append = append;
			Identity = identity;
		}

		public Monoid(Func<M, M, M> sum, M identity)
		{
			Identity = identity;
			_append = x => y => sum(x, y);
		}

		public Monoid(Semigroup<M> semigroup, M identity)
		{
			_append = semigroup.Sum();
			Identity = identity;
		}

		public Monoid<Func<N, M>> Map<N>(Monoid<N> n)
		{
			return new Monoid<Func<N, M>>(Semigroup().Map<N>(), x => Identity);
		}

		public M Append(M left, M right)
		{
			return _append(left)(right);
		}

		Func<M, M> Append(M value)
		{
			return _append(value);
		}

		Func<M, Func<M, M>> Append()
		{
			return _append;
		}

		Semigroup<M> Semigroup()
		{
			return new Semigroup<M>(_append);
		}
	}
}