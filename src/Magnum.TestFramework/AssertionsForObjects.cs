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
	using NUnit.Framework;

	public static class AssertionsForObjects
	{
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


		public static T ShouldBeAnInstanceOf<T>(this object value)
		{
			Assert.IsInstanceOf<T>(value);

			return (T)value;
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