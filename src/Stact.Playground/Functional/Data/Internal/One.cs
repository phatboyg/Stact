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
namespace Stact.Functional.Data.Internal
{
	using System;


	public class One<T, M> :
		Digit<T, M>
	{
		readonly Element<T, M> _v;


		public One(Measured<T, M> m, Element<T, M> v)
			: base(m, v.Size)
		{
			_v = v;
		}

		public Element<T, M> V
		{
			get { return _v; }
		}

		public override Element<T, M> Left
		{
			get { return _v; }
		}

		public override Element<T, M> Right
		{
			get { return _v; }
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return f(_v.Value)(z);
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return f(z)(_v.Value);
		}

		public override U Match<U>(Func<One<T, M>, U> one, Func<Two<T, M>, U> two, Func<Three<T, M>, U> three,
		                           Func<Four<T, M>, U> four)
		{
			return one(this);
		}
	}
}