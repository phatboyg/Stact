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

	public abstract class FingerTree<T, M>
	{
		public readonly Measured<T, M> Measured;
		public readonly M Size;

		protected FingerTree(Measured<T, M> measured, M size)
		{
			Measured = measured;
			Size = size;
		}

		public bool IsEmpty
		{
			get
			{
				return Match(e => true, s => false, d => false);
			}
		}

		public abstract LeftView<T,M> Left { get; }

		public abstract RightView<T, M> Right { get; }

		public abstract U FoldRight<U>(Func<T, Func<U, U>> f, U z);
		public abstract T ReduceRight(Func<T, Func<T, T>> f);
		public abstract U FoldLeft<U>(Func<U, Func<T, U>> f, U z);
		public abstract T ReduceLeft(Func<T, Func<T, T>> f);

		public abstract FingerTree<U, M> Map<U>(Func<T, U> f, Measured<U, M> m);

		public abstract U Match<U>(Func<Empty<T, M>, U> empty, Func<Single<T, M>, U> single, Func<Deep<T, M>, U> deep);

		public abstract bool Visit(Func<T, bool> callback);

		public static MakeTree<T, M> MakeTree(Measured<T, M> m)
		{
			return new MakeTree<T, M>(m);
		}

		public abstract FingerTree<T, M> AddLeft(T a);
		public abstract FingerTree<T, M> AddRight(T a);

		public abstract FingerTree<T, M> Concat(FingerTree<T, M> t);

		public abstract Pair<FingerTree<T, M>, FingerTree<T, M>> SplitSequence(MeasurePredicate<M> predicate);
		public abstract Split<T, FingerTree<T, M>, M> Split(MeasurePredicate<M> predicate, M acc);
	}
}