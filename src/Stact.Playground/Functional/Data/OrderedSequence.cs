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
	using System.Collections.Generic;
	using Internal;


	public class OrderedSequence<T, M>
		where M : IComparable<M>
	{
		Measured<T, M> _measured;
		MakeTree<T, M> _mk;
		FingerTree<T, M> _tree;

		OrderedSequence(FingerTree<T, M> tree)
		{
			_measured = tree.Measured;
			_mk = FingerTree<T, M>.MakeTree(_measured);
			_tree = tree;
		}

		public bool IsEmpty
		{
			get { return _tree.IsEmpty; }
		}

		public static OrderedSequence<T, M> Empty(Monoid<M> monoid, Func<T, M> measure)
		{
			var measured = new Measured<T, M>(monoid, measure);
			return new OrderedSequence<T, M>(FingerTree<T, M>.MakeTree(measured).Empty());
		}

		Pair<OrderedSequence<T, M>, OrderedSequence<T, M>> Partition(M vK)
		{
			Pair<FingerTree<T, M>, FingerTree<T, M>> split = _tree.SplitSequence(y => vK.CompareTo(y) < 0);

			var left = new OrderedSequence<T, M>(split.First);
			var right = new OrderedSequence<T, M>(split.Second);

			return Pair.New(left, right);
		}

		public OrderedSequence<T, M> Insert(T value)
		{
			Pair<OrderedSequence<T, M>, OrderedSequence<T, M>> part = Partition(_measured.Measure(value));

			return new OrderedSequence<T, M>(part.First._tree.AddRight(value).Concat(part.Second._tree));
		}

		public bool Visit(Func<T, bool> callback)
		{
			return _tree.Visit(callback);
		}

		public T[] ToArray()
		{
			var list = new List<T>();
			var view = _tree.Left;
			while(view != null)
			{
				list.Add(view.Head);
				view = view.Tail.Left;
			}

			return list.ToArray();
		}
	}
}