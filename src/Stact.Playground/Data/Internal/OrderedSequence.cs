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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using FingerTree;


	public class OrderedSequence<T, M> :
		IEnumerable<T>
		where M : IComparable
	{
		readonly Key<T, M> _key;
		readonly FTreeM<OrderedElement<T, M>, M> _tree;

		public OrderedSequence(Key<T, M> key)
		{
			_key = key;
			_tree = new EmptyFTreeM<OrderedElement<T, M>, M>(new KeyMonoid<T, M>(key).Monoid);
		}

		public OrderedSequence(Key<T, M> key, IEnumerable<T> elements)
		{
			_key = key;

			_tree = elements
//				.Select(x => new OrderedElement<T, M>(x, key))
				.Aggregate(new OrderedSequence<T, M>(key), (current, element) => current.Insert(element))
				._tree;
		}

		protected OrderedSequence(Key<T, M> key, FTreeM<OrderedElement<T, M>, M> source)
		{
			_key = key;
			_tree = source;
		}

		static bool LessThan2(M v1, M v2)
		{
			return v1.CompareTo(v2) < 0;
		}

		static bool LessThanOrEqual2(M v1, M v2)
		{
			return v1.CompareTo(v2) <= 0;
		}

		static bool GreaterThan2(M v1, M v2)
		{
			return v1.CompareTo(v2) > 0;
		}


		public OrderedSequence<T, M> PushBack(OrderedElement<T, M> ordEl)
		{
			ViewR<OrderedElement<T, M>, M> viewR = _tree.RightView();

			if (viewR != null)
			{
				if (viewR.last.Measure()
				    	.CompareTo(ordEl.Measure())
				    > 0)
				{
					throw new Exception(
						"OrderedSequence Error: PushBack() of an element less than the biggest seq el."
						);
				}
			}
			//else
			return new OrderedSequence<T, M>(_key, _tree.PushBack(ordEl));
		}

		public OrderedSequence<T, M> PushFront(OrderedElement<T, M> ordEl)
		{
			ViewL<OrderedElement<T, M>, M> viewL = _tree.LeftView();

			if (viewL != null)
			{
				if (_tree.LeftView().head.Measure()
				    	.CompareTo(ordEl.Measure())
				    < 0)
				{
					throw new Exception(
						"OrderedSequence Error: PushFront() of an element greater than the smallest seq el."
						);
				}
			}
			//else
			return new OrderedSequence<T, M>(_key, _tree.PushFront(ordEl));
		}

		Pair<OrderedSequence<T, M>, OrderedSequence<T, M>> Partition(M vK)
		{
			Pair<FTreeM<OrderedElement<T, M>, M>, FTreeM<OrderedElement<T, M>, M>> baseSeqSplit=_tree.SeqSplit(new MPredicate<M>(FP.Curry<M, M, bool>(LessThanOrEqual2, vK)));

			var left= new OrderedSequence<T, M>(_key, baseSeqSplit.First);

			var right= new OrderedSequence<T, M>(_key, baseSeqSplit.Second);

			return new Pair<OrderedSequence<T, M>,OrderedSequence<T, M>>(left, right);
		}

		public OrderedSequence<T, M> Insert(T value)
		{
			Pair<OrderedSequence<T, M>, OrderedSequence<T, M>> tPart = Partition(_key.Accessor(value));

			var element = new OrderedElement<T, M>(value, _key);

			return new OrderedSequence<T, M>(_key, tPart.First._tree.Merge(tPart.Second._tree.PushFront(element)));
		}

		public OrderedSequence<T, M> DeleteAll(T t)
		{
			M vK = _key.Accessor(t); // the Key of t

			Pair<OrderedSequence<T, M>, OrderedSequence<T, M>> tPart = Partition(vK);

			OrderedSequence<T, M> seqPrecedestheEl = tPart.First;
			OrderedSequence<T, M> seqStartsWiththeEl = tPart.Second;

			Pair<FTreeM<OrderedElement<T, M>, M>, FTreeM<OrderedElement<T, M>, M>> lastTreeSplit
				=seqStartsWiththeEl._tree.SeqSplit(new MPredicate<M>(FP.Curry<M, M, bool>(LessThan2, vK)));

			//OrderedSequence<T, V> seqBeyondtheEl =
			//    new OrderedSequence<T, V>(KeyObj, lastTreeSplit.second);

			return new OrderedSequence<T, M>(_key,seqPrecedestheEl._tree.Merge(lastTreeSplit.Second));
		}

		public OrderedSequence<T, M> Merge(OrderedSequence<T, M> other)
		{
			FTreeM<OrderedElement<T, M>, M> merged = OrdMerge(_tree, other._tree);

			return new OrderedSequence<T, M>(_key, merged);
		}

		static FTreeM<OrderedElement<T, M>, M>OrdMerge(FTreeM<OrderedElement<T, M>, M> ordTree1,FTreeM<OrderedElement<T, M>, M> ordTree2)
		{
			ViewL<OrderedElement<T, M>, M> lView2 = ordTree2.LeftView();

			if (lView2 == null)
				return ordTree1;
			//else
			OrderedElement<T, M> bHead = lView2.head;
			FTreeM<OrderedElement<T, M>, M> bTail = lView2.ftTail;

			// Split ordTree1 on elems <= and then > bHead
			Pair<FTreeM<OrderedElement<T, M>, M>, FTreeM<OrderedElement<T, M>, M>>
				tree1Split = ordTree1.SeqSplit
					(new MPredicate<M>
					 	(FP.Curry<M, M, bool>(LessThanOrEqual2, bHead.Measure()))
					);

			FTreeM<OrderedElement<T, M>, M> leftTree1 = tree1Split.First;
			FTreeM<OrderedElement<T, M>, M> rightTree1 = tree1Split.Second;

			// OrdMerge the tail of ordTree2 
			//          with the right-split part of ordTree1
			FTreeM<OrderedElement<T, M>, M>
				mergedRightparts = OrdMerge(bTail, rightTree1);

			return leftTree1.Merge(mergedRightparts.PushFront(bHead));
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (var element in _tree.ToSequence())
			{
				yield return element.Value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}