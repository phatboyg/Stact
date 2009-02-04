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

	public class Identity<T>
	{
		public Identity(T value)
		{
			Value = value;
		}

		public T Value { get; private set; }
	}

	public static class ExtensionsToIdentity
	{
		public static Identity<T> ToIdentity<T>(this T value)
		{
			return new Identity<T>(value);
		}

		public static Identity<U> SelectMany<T, U>(this Identity<T> id, Func<T, Identity<U>> k)
		{
			return k(id.Value);
		}

		public static Identity<V> SelectMany<T, U, V>(this Identity<T> id, Func<T, Identity<U>> k, Func<T, U, V> s)
		{
			return s(id.Value, k(id.Value).Value).ToIdentity();
		}
	}
}