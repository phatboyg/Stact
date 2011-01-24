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
		readonly Element<T, M> _v1;
		readonly Element<T, M> _v2;
		readonly Element<T, M> _v3;

		public Node3(Measured<T, M> m, Element<T, M> v1, Element<T, M> v2, Element<T, M> v3)
			: base(m, m.Append(v1.Size, m.Append(v2.Size, v3.Size)))
		{
			_v1 = v1;
			_v2 = v2;
			_v3 = v3;
		}

		public Element<T, M> V1
		{
			get { return _v1; }
		}

		public Element<T, M> V2
		{
			get { return _v2; }
		}

		public Element<T, M> V3
		{
			get { return _v3; }
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return f(_v1.Value)(f(_v2.Value)(f(_v3.Value)(z)));
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return f(f(f(z)(_v1.Value))(_v2.Value))(_v3.Value);
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