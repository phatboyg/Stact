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


	public abstract class Digit<T, M>
	{
		public readonly M Size;
		readonly Measured<T, M> _m;
		readonly MakeTree<T, M> _mk;

		protected Digit(Measured<T, M> m, M size)
		{
			Size = size;
			_m = m;
			_mk = new MakeTree<T, M>(m);
		}

		public M Measure()
		{
			return FoldLeft(x => t => _m.Append(x, _m.Measure(t)), _m.Identity);
		}

		public abstract bool Visit(Func<T, bool> callback);
		public abstract U FoldRight<U>(Func<T, Func<U, U>> f, U z);
		public abstract U FoldLeft<U>(Func<U, Func<T, U>> f, U z);

		public abstract T Left { get; }
		public abstract T Right { get; }

		public T ReduceRight(Func<T, Func<T, T>> f)
		{
			return Match(one => one.V,
			             two => f(two.V1)(two.V2),
			             three => f(three.V1)(f(three.V2)(three.V3)),
			             four => f(four.V1)(f(four.V2)(f(four.V3)(four.V4))));
		}

		public T ReduceLeft(Func<T, Func<T, T>> f)
		{
			return Match(one => one.V,
			             two => f(two.V2)(two.V1),
			             three => f(three.V3)(f(three.V2)(three.V1)),
			             four => f(four.V4)(f(four.V3)(f(four.V2)(four.V1))));
		}

		public Split<T, Digit<T, M>, M> Split(MeasurePredicate<M> predicate, M acc)
		{
			return Match(one => new Split<T, Digit<T, M>, M>(null, one.V, null),
			             two =>
			             	{
			             		M value = _m.Append(acc, _m.Measure(two.V1));
			             		if (predicate(value))
			             			return new Split<T, Digit<T, M>, M>(null, two.V1, _mk.One(two.V2));

			             		return new Split<T, Digit<T, M>, M>(_mk.One(two.V1), two.V2, null);
			             	},
			             three =>
			             	{
			             		M value = _m.Append(acc, _m.Measure(three.V1));
			             		if (predicate(value))
			             			return new Split<T, Digit<T, M>, M>(null, three.V1, _mk.Two(three.V2, three.V3));
			             		value = _m.Append(value, _m.Measure(three.V2));
			             		if (predicate(value))
			             			return new Split<T, Digit<T, M>, M>(_mk.One(three.V1), three.V2, _mk.One(three.V3));

			             		throw new InvalidOperationException("Should not have split prefix if not in the middle");
			             	},
			             four =>
			             	{
			             		M value = _m.Append(acc, _m.Measure(four.V1));
			             		if (predicate(value))
			             			return new Split<T, Digit<T, M>, M>(null, four.V1, _mk.Three(four.V2, four.V3, four.V4));
			             		value = _m.Append(value, _m.Measure(four.V2));
			             		if (predicate(value))
			             			return new Split<T, Digit<T, M>, M>(_mk.One(four.V1), four.V2, _mk.Two(four.V3, four.V4));
			             		value = _m.Append(value, _m.Measure(four.V3));
			             		if (predicate(value))
			             			return new Split<T, Digit<T, M>, M>(_mk.Two(four.V1, four.V2), four.V3, _mk.One(four.V4));

			             		throw new InvalidOperationException("Should not have split prefix if not in the middle");
			             	});
		}

		public Digit<U, M> Map<U>(Func<T, U> f, Measured<U, M> m)
		{
			return Match<Digit<U, M>>(one => new One<U, M>(m, f(one.V)),
			                          two => new Two<U, M>(m, f(two.V1), f(two.V2)),
			                          three => new Three<U, M>(m, f(three.V1), f(three.V2), f(three.V3)),
			                          four => new Four<U, M>(m, f(four.V1), f(four.V2), f(four.V3), f(four.V4)));
		}

		public FingerTree<T, M> ToTree()
		{
			return Match(one => _mk.Single(one.V),
			             two => _mk.Deep(_mk.One(two.V1), new Empty<Node<T, M>, M>(_m.Node), _mk.One(two.V2)),
			             three => _mk.Deep(_mk.Two(three.V1, three.V2), new Empty<Node<T, M>, M>(_m.Node), _mk.One(three.V3)),
			             four =>
			             _mk.Deep(_mk.Two(four.V1, four.V2), new Empty<Node<T, M>, M>(_m.Node), _mk.Two(four.V3, four.V4)));
		}

		public abstract U Match<U>(Func<One<T, M>, U> one, Func<Two<T, M>, U> two, Func<Three<T, M>, U> three,
		                           Func<Four<T, M>, U> four);
	}
}