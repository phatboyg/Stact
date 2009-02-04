// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Monads
{
	using System;

	public delegate void K<T>(Action<T> c);

	public static class Continuations
	{
		public static K<T> ToContinuation<T>(this T value)
		{
			return c => c(value);
		}

		public static K<U> SelectMany<T, U>(this K<T> m, Func<T, K<U>> k)
		{
			return c => m(x => k(x)(c));
		}

		public static K<V> SelectMany<T, U, V>(this K<T> m, Func<T, K<U>> k, Func<T, U, V> s)
		{
			return m.SelectMany(x => k(x).SelectMany(y => s(x, y).ToContinuation()));
		}

		public static K<U> Select<U, T>(this K<T> m, Func<T, U> k)
		{
			return c => m(x => c(k(x)));
		}
	}
}