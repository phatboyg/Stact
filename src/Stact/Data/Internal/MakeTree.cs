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
	public class MakeTree<T, M>
	{
		readonly Measured<T, M> _m;

		public MakeTree(Measured<T, M> m)
		{
			_m = m;
		}

		public FingerTree<T, M> Empty()
		{
			return new Empty<T, M>(_m);
		}

		public FingerTree<T, M> Single(T item)
		{
			return new Single<T, M>(_m, item);
		}

		public FingerTree<T, M> Deep(Digit<T, M> prefix, FingerTree<Node<T, M>, M> middle, Digit<T, M> suffix)
		{
			return Deep(_m.Append(prefix.Measure(), _m.Append(middle.Size, suffix.Measure())), prefix, middle, suffix);
		}

		public FingerTree<T, M> Deep(M measure, Digit<T, M> prefix, FingerTree<Node<T, M>, M> middle, Digit<T, M> suffix)
		{
			return new Deep<T, M>(_m, measure, prefix, middle, suffix);
		}

		public One<T, M> One(T item)
		{
			return new One<T, M>(_m, item);
		}

		public Two<T, M> Two(T item1, T item2)
		{
			return new Two<T, M>(_m, item1, item2);
		}

		public Three<T, M> Three(T item1, T item2, T item3)
		{
			return new Three<T, M>(_m, item1, item2, item3);
		}

		public Four<T, M> Four(T item1, T item2, T item3, T item4)
		{
			return new Four<T, M>(_m, item1, item2, item3, item4);
		}

		public Node2<T, M> Node2(T v1, T v2)
		{
			return new Node2<T, M>(_m, v1, v2);
		}

		public Node2<T, M> Node2(Node2<T, M> other)
		{
			return new Node2<T, M>(_m, other);
		}

		public Node3<T, M> Node3(T v1, T v2, T v3)
		{
			return new Node3<T, M>(_m, v1, v2, v3);
		}

		public Node3<T, M> Node3(Node3<T, M> other)
		{
			return new Node3<T, M>(_m, other);
		}
	}
}