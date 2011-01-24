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


	public class Empty<T, M> :
		FingerTree<T, M>
	{
		public Empty(Measured<T, M> m)
			: base(m, m.Identity)
		{
		}

		public override FingerTree<T, M> cons(T a)
		{
			return new Single<T, M>(Measured, a);
		}

		public override FingerTree<T, M> snoc(T a)
		{
			return cons(a);
		}

		public override FingerTree<T, M> append(FingerTree<T, M> t)
		{
			return t;
		}

		public override Func<T> Lookup(Func<M, int> o, int i)
		{
			throw new NotImplementedException("Tree is empty");
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return z;
		}

		public override T ReduceRight(Func<T, Func<T, T>> f)
		{
			throw new NotImplementedException("Tree is empty");
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return z;
		}

		public override T ReduceLeft(Func<T, Func<T, T>> f)
		{
			throw new NotImplementedException("Tree is empty");
		}

		public override Split<T, FingerTree<T, M>, M> Split(MeasurePredicate<M> predicate, M acc)
		{
			throw new NotImplementedException("Tree is empty");
		}

		public override Pair<FingerTree<T, M>, FingerTree<T, M>> SplitSequence(MeasurePredicate<M> predicate)
		{
			return Pair.New<FingerTree<T, M>, FingerTree<T, M>>(new Empty<T, M>(Measured), new Empty<T, M>(Measured));
		}

		public override FingerTree<U, M> Map<U>(Func<T, U> f, Measured<U, M> m)
		{
			return new Empty<U, M>(m);
		}

		public override U Match<U>(Func<Empty<T, M>, U> empty, Func<Single<T, M>, U> single, Func<Deep<T, M>, U> deep)
		{
			return empty(this);
		}

		public override bool Visit(Func<T, bool> callback)
		{
			return true;
		}
	}
}