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
namespace Stact
{
	using System;


	public static class ExtensionsToMaybe
	{
		// Unit
		public static Maybe<T> ToMaybe<T>(this T value)
		{
			return new Maybe<T>(value);
		}

		public static Maybe<string> ToMaybe(this string value)
		{
			if (value == null)
				return Maybe<string>.Nothing;

			return new Maybe<string>(value);
		}

		// Bind
		public static Maybe<U> SelectMany<T, U>(this Maybe<T> m, Func<T, Maybe<U>> k)
		{
			return m.HasValue ? k(m.Value) : Maybe<U>.Nothing;
		}

		public static Maybe<U> SelectMany<T, U>(this Maybe<T> m, Func<T, Identity<U>> k)
		{
			return m.HasValue ? k(m.Value).Value.ToMaybe() : Maybe<U>.Nothing;
		}

		public static Maybe<U> SelectMany<T, U>(this Maybe<T> m, Func<T, U> k)
		{
			return m.HasValue ? k(m.Value).ToMaybe() : Maybe<U>.Nothing;
		}

//		public static Maybe<V> SelectMany<T, U, V>(this Maybe<T> m, Func<T, U> k, Func<T, U, V> s)
//		{
//			return m.SelectMany(x => k(x).ToMaybe().SelectMany(y => s(x, y).ToMaybe()));
//		}

		public static Maybe<V> SelectMany<T, U, V>(this Maybe<T> m, Func<T, Identity<U>> k, Func<T, U, V> s)
		{
			return m.SelectMany(x => k(x).Value.ToMaybe().SelectMany(y => s(x, y).ToMaybe()));
		}

		public static Maybe<V> SelectMany<T, U, V>(this Maybe<T> m, Func<T, Maybe<U>> k, Func<T, U, V> s)
		{
			return m.SelectMany(x => k(x).SelectMany(y => s(x, y).ToMaybe()));
		}

		public static Maybe<V> SelectMany<T, U, V>(this Maybe<T> m, Func<T, Maybe<U>> k, Func<T, U, Maybe<V>> s)
		{
			return m.SelectMany(x => k(x).SelectMany(y => s(x, y)));
		}
	}
}