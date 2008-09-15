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
namespace Magnum.Common.ObjectExtensions
{
	using System;

	public static class ObjectExt
	{
		public static void MustNotBeNull<T>(this T reference) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException();
		}

		public static void MustNotBeNull<T>(this T reference, string name) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException(name);
		}

		public static void MustNotBeNull<T>(this T reference, string name, string message) where T : class
		{
			if (reference == null)
				throw new ArgumentNullException(name, message);
		}

		public static void MustNotBeEmpty(this string value)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException();
		}

		public static void MustNotBeEmpty(this string value, string name)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("The argument must not be empty", name);
		}

		public static void MustNotBeEmpty(this string value, string name, string message)
		{
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException(message, name);
		}

		public static void MustBeInRange<T>(this T value, RangeBuilder<T> rangeBuilder)
		{
			Range<T> range = rangeBuilder;

			value.MustBeInRange(range);
		}

		public static void MustBeInRange<T>(this T value, Range<T> range)
		{
			if (!range.Contains(value))
				throw new ArgumentException();
		}
	}
}