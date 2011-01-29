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


	public class Node3<T, M> :
		Node<T, M>
	{
		readonly T _v1;
		readonly T _v2;
		readonly T _v3;

		public Node3(Measured<T, M> m, T v1, T v2, T v3)
			: base(m, m.Append(m.Measure(v1), m.Append(m.Measure(v2), m.Measure(v3))))
		{
			_v1 = v1;
			_v2 = v2;
			_v3 = v3;
		}

		public Node3(Measured<T, M> m, Node3<T, M> other)
			: base(m, m.Append(m.Measure(other.V1), m.Append(m.Measure(other.V2), m.Measure(other.V3))))
		{
			_v1 = other.V1;
			_v2 = other.V2;
			_v3 = other.V3;
		}

		public T V1
		{
			get { return _v1; }
		}

		public T V2
		{
			get { return _v2; }
		}

		public T V3
		{
			get { return _v3; }
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return f(_v1)(f(_v2)(f(_v3)(z)));
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return f(f(f(z)(_v1))(_v2))(_v3);
		}

		public override bool Visit(Func<T, bool> callback)
		{
			return callback(_v1) && callback(_v2) && callback(_v3);
		}

		public override Digit<T, M> ToDigit()
		{
			return new Three<T, M>(Measured, _v1, _v2, _v3);
		}

		public override U Match<U>(Func<Node2<T, M>, U> node2, Func<Node3<T, M>, U> node3)
		{
			return node3(this);
		}
	}
}