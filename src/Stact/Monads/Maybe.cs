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
namespace Stact.Monads
{
	using System;

	public class Maybe<T>
	{
		public static readonly Maybe<T> Nothing = new Maybe<T>();

		private Maybe()
		{
			HasValue = false;
		}

		public Maybe(T value)
		{
			Value = value;
			HasValue = true;
		}

		public T Value { get; private set; }
		public bool HasValue { get; private set; }
	}

	public static class ExtensionsToMaybe
	{
		public static Maybe<T> ToMaybe<T>(this T value)
		{
			return new Maybe<T>(value);
		}

//		public static Maybe<V> SeleaactMany<T, U, V>(this Maybe<T> m, Func<T, Maybe<U>> k, Func<T, U, V> s)
//		{
//			return s(m.Value, !m.HasValue ? Maybe<U>.Nothing.Value : k(m.Value).Value).ToMaybe();
//		}

		public static Maybe<V> SelectMany<T, U, V>(this Maybe<T> m, Func<T, Maybe<U>> k, Func<T, U, V> s)
		{
			return m.SelectMany(x => k(x).SelectMany(y => s(x, y).ToMaybe()));

			//return s(m.Value, m.HasValue ? k(m.Value).Value : Maybe<U>.Nothing).ToMaybe();
		}

		public static Maybe<U> SelectMany<T, U>(this Maybe<T> m, Func<T, Maybe<U>> k)
		{
			return !m.HasValue ? Maybe<U>.Nothing : k(m.Value);
		}
	}
}