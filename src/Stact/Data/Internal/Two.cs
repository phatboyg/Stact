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


	public class Two<T, M> :
		Digit<T, M>
	{
		readonly T _v1;
		readonly T _v2;

		public Two(Measured<T, M> m, T v1, T v2)
			: base(m, m.Append(m.Measure(v1), m.Measure(v2)))
		{
			_v1 = v1;
			_v2 = v2;
		}

		public T V1
		{
			get { return _v1; }
		}

		public T V2
		{
			get { return _v2; }
		}

		public override bool Visit(Func<T, bool> callback)
		{
			return callback(_v1) && callback(_v2);
		}

		public override U FoldRight<U>(Func<T, Func<U, U>> f, U z)
		{
			return f(_v1)(f(_v2)(z));
		}

		public override U FoldLeft<U>(Func<U, Func<T, U>> f, U z)
		{
			return f(f(z)(_v2))(_v1);
		}

		public override U Match<U>(Func<One<T, M>, U> one, Func<Two<T, M>, U> two, Func<Three<T, M>, U> three,
		                           Func<Four<T, M>, U> four)
		{
			return two(this);
		}
	}
}