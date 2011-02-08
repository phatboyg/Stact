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
namespace Stact.Functional.Data.Internal
{
	using System;


	public class Single<T, M> :
		FingerTree<T, M>
	{
		readonly Element<T, M> _item;
		readonly MakeTree<T, M> _mk;

		public Single(Measured<T, M> m, Element<T,M> item)
			: base(m, item.Size)
		{
			_item = item;
			_mk = new MakeTree<T, M>(m);
		}

		public Element<T, M> Item
		{
			get { return _item; }
		}

		public override LeftView<T, M> Left
		{
			get { return new LeftView<T, M>(_item, _mk.Empty()); }
		}

		public override RightView<T, M> Right
		{
			get { return new RightView<T, M>(_item, _mk.Empty()); }
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return f(_item.Value)(z);
		}

		public override T ReduceRight(Func<T, Func<T, T>> f)
		{
			return _item.Value;
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return f(z)(_item.Value);
		}

		public override T ReduceLeft(Func<T, Func<T, T>> f)
		{
			return _item.Value;
		}

		public override FingerTree<U, M> Map<U>(Func<T, U> f, Measured<U, M> m)
		{
			var value = f(_item.Value);

			return new Single<U, M>(m, m.Measure(value));
		}

		public override U Match<U>(Func<Empty<T, M>, U> empty, Func<Single<T, M>, U> single, Func<Deep<T, M>, U> deep)
		{
			return single(this);
		}

		public override FingerTree<T, M> AddLeft(Element<T,M> a)
		{
			return _mk.Deep(_mk.One(a), new Empty<Node<T, M>, M>(Measured.Node), _mk.One(_item));
		}

		public override FingerTree<T, M> AddRight(Element<T, M> a)
		{
			return _mk.Deep(_mk.One(_item), new Empty<Node<T, M>, M>(Measured.Node), _mk.One(a));
		}

		public override FingerTree<T, M> Concat(FingerTree<T, M> t)
		{
			return t.AddLeft(_item);
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