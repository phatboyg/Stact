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


	public class Sequence<T>
	{
		static Measured<T, int> _measured = new Measured<T, int>(Monoid.IntAdd, x => 1);
		static MakeTree<T, int> _mk = FingerTree<T, int>.MakeTree(_measured);
		FingerTree<T, int> _tree;

		Sequence(FingerTree<T, int> tree)
		{
			_tree = tree;
		}

		public bool IsEmpty
		{
			get { return _tree.IsEmpty; }
		}

		public int Count
		{
			get { return _tree.Size; }
		}

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException("index");

				throw new NotImplementedException("hmmmm");
			}
		}

		public static Sequence<T> Empty()
		{
			return new Sequence<T>(_mk.Empty());
		}

		public static Sequence<T> Single(T item)
		{
			return new Sequence<T>(_mk.Single(item));
		}

		public Sequence<T> PushFront(T item)
		{
			return new Sequence<T>(_tree.AddLeft(item));
		}

		public Sequence<T> PushBack(T item)
		{
			return new Sequence<T>(_tree.AddRight(item));
		}

		public Sequence<T> Append(Sequence<T> other)
		{
			return new Sequence<T>(_tree.Concat(other._tree));
		}

		public bool Visit(Func<T, bool> callback)
		{
			return _tree.Visit(callback);
		}
	}
}