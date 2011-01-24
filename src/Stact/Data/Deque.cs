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
	using Internal;


	public class Deque<T>
	{
		Measured<T, int> _measured;
		FingerTree<T, int> _tree;

		public Deque()
		{
			_measured = new Measured<T, int>(Monoid.IntAdd, x => 1);

			_tree = FingerTree<T, int>.MakeTree(_measured).Empty();
		}

		Deque(Measured<T, int> measured, FingerTree<T, int> tree)
		{
			_measured = measured;
			_tree = tree;
		}

		public Deque<T> AddHead(T item)
		{
			return new Deque<T>(_measured, _tree.AddLeft(_measured.Measure(item)));
		}

		public Deque<T> AddTail(T item)
		{
			return new Deque<T>(_measured, _tree.AddRight(_measured.Measure(item)));
		}

		public Deque<T> RemoveHead(out T item)
		{
			LeftView<T, int> view = _tree.Left;

			item = view != null ? view.Head.Value : default(T);

			return new Deque<T>(_measured, view.Tail);
		}
	}
}