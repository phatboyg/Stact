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


	public class Single<T, M> :
		FingerTree<T, M>
	{
		readonly T _item;
		readonly MakeTree<T, M> _mk;

		public Single(Measured<T, M> m, T item)
			: base(m, m.Measure(item))
		{
			_item = item;
			_mk = new MakeTree<T, M>(m);
		}

		public T Item
		{
			get { return _item; }
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return f(_item)(z);
		}

		public override T ReduceRight(Func<T, Func<T, T>> f)
		{
			return _item;
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return f(z)(_item);
		}

		public override T ReduceLeft(Func<T, Func<T, T>> f)
		{
			return _item;
		}

		public override FingerTree<U, M> Map<U>(Func<T, U> f, Measured<U, M> m)
		{
			return new Single<U, M>(m, f(_item));
		}

		public override U Match<U>(Func<Empty<T, M>, U> empty, Func<Single<T, M>, U> single, Func<Deep<T, M>, U> deep)
		{
			return single(this);
		}

		public override bool Visit(Func<T, bool> callback)
		{
			return callback(_item);
		}

		public override FingerTree<T, M> cons(T a)
		{
			return _mk.Deep(_mk.One(a), new Empty<Node<T, M>, M>(Measured.Node), _mk.One(_item));
		}

		public override FingerTree<T, M> snoc(T a)
		{
			return _mk.Deep(_mk.One(_item), new Empty<Node<T, M>, M>(Measured.Node), _mk.One(a));
		}

		public override FingerTree<T, M> append(FingerTree<T, M> t)
		{
			return t.cons(_item);
		}

		public override Func<T> Lookup(Func<M, int> o, int i)
		{
			return () => _item;
		}

		public override Pair<FingerTree<T, M>, FingerTree<T, M>> SplitSequence(MeasurePredicate<M> predicate)
		{
			if (predicate(Size))
				return new Pair<FingerTree<T, M>, FingerTree<T, M>>(new Empty<T, M>(Measured), this);

			return new Pair<FingerTree<T, M>, FingerTree<T, M>>(this, new Empty<T, M>(Measured));
		}

		public override Split<T, FingerTree<T, M>, M> Split(MeasurePredicate<M> predicate, M acc)
		{
			return new Split<T, FingerTree<T, M>, M>(new Empty<T, M>(Measured), _item, new Empty<T, M>(Measured));
		}
	}
}