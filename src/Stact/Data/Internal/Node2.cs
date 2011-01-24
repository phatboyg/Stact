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


	public class Node2<T, M> :
		Node<T, M>
	{
		readonly T _v1;
		readonly T _v2;

		public Node2(Measured<T, M> m, T v1, T v2)
			: base(m, m.Append(m.Measure(v1), m.Measure(v2)))
		{
			_v1 = v1;
			_v2 = v2;
		}

		public Node2(Measured<T, M> m, Node2<T, M> other)
			: base(m, m.Append(m.Measure(other.V1), m.Measure(other.V2)))
		{
			_v1 = other.V1;
			_v2 = other.V2;
		}

		public T V1
		{
			get { return _v1; }
		}

		public T V2
		{
			get { return _v2; }
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return f(_v1)(f(_v2)(z));
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return f(f(z)(_v1))(_v2);
		}

		public override bool Visit(Func<T, bool> callback)
		{
			return callback(_v1) && callback(_v2);
		}

		public override Digit<T, M> ToDigit()
		{
			return new Two<T, M>(Measured, _v1, _v2);
		}

		public override U Match<U>(Func<Node2<T, M>, U> node2, Func<Node3<T, M>, U> node3)
		{
			return node2(this);
		}
	}
}