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
namespace Magnum.TestFramework
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using NUnit.Framework;

	public static class AssertionsForObjects
	{
		public static bool ShouldBeTrue(this bool value)
		{
			Assert.IsTrue(value);

			return value;
		}

		public static bool ShouldBeFalse(this bool value)
		{
			Assert.IsFalse(value);

			return value;
		}

		public static T ShouldBeNull<T>(this T instance)
			where T : class
		{
			Assert.IsNull(instance);

			return instance;
		}

		public static T ShouldNotBeNull<T>(this T instance)
			where T : class
		{
			Assert.IsNotNull(instance);

			return instance;
		}

		public static T ShouldEqual<T>(this T value, T expected)
		{
			Assert.AreEqual(expected, value);

			return value;
		}

		public static int ShouldBeGreaterThan(this int value, int expected)
		{
			Assert.Greater(value, expected);

			return value;
		}

		public static int ShouldBeGreaterThanOrEqualTo(this int value, int expected)
		{
			Assert.GreaterOrEqual(value, expected);

			return value;
		}

		public static int ShouldBeLessThan(this int value, int expected)
		{
			Assert.Less(value, expected);

			return value;
		}

		public static int ShouldBeLessThanOrEqualTo(this int value, int expected)
		{
			Assert.LessOrEqual(value, expected);

			return value;
		}

		public static T ShouldNotEqual<T>(this T value, T expected)
		{
			Assert.AreNotEqual(expected, value);

			return value;
		}

		public static T ShouldBeTheSameAs<T>(this T value, T expected)
		{
			Assert.AreSame(expected, value);

			return value;
		}

		public static T ShouldNotBeTheSameAs<T>(this T value, T expected)
		{
			Assert.AreNotSame(expected, value);

			return value;
		}

		public static ICollection<T> ShouldBeEmpty<T>(this ICollection<T> collection)
		{
			Assert.AreEqual(0, collection.Count);

			return collection;
		}

		public static ICollection<T> ShouldNotBeEmpty<T>(this ICollection<T> collection)
		{
			Assert.Greater(collection.Count, 0);

			return collection;
		}

		public static ICollection ShouldBeEmpty<T>(this ICollection collection)
		{
			Assert.IsEmpty(collection);

			return collection;
		}

		public static ICollection ShouldNotBeEmpty<T>(this ICollection collection)
		{
			Assert.IsNotEmpty(collection);

			return collection;
		}

		public static object ShouldBeAnInstanceOf<T>(this object value)
		{
			Assert.IsInstanceOf<T>(value);

			return value;
		}

		public static object ShouldBeAssignableFrom<T>(this object value)
		{
			Assert.IsAssignableFrom<T>(value);

			return value;
		}

		public static void ShouldThrow<T>(this TestDelegate action)
			where T : Exception
		{
			Assert.Throws<T>(action);
		}
	}
}