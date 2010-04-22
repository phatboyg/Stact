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

	public static class AssertionsForString
	{
		public static string ShouldNotEqual(this string value, string expected)
		{
			Assert.AreNotEqual(expected, value);

			return value;
		}

		public static string ShouldEqual(this string value, string expected)
		{
			Assert.AreEqual(expected, value);

			return value;
		}

		public static string ShouldMatchIgnoringCase(this string value, string expected)
		{
			if (value == null && expected == null)
				return null;

			Assert.IsNotNull(value);

			Assert.IsTrue(value.Equals(expected, StringComparison.InvariantCultureIgnoreCase), "Expected: '{0}', Found: '{1}'", expected, value);

			return value;
		}

		public static string ShouldBeEmpty(this string value)
		{
			Assert.IsEmpty(value);

			return value;
		}

		public static string ShouldNotBeEmpty(this string value)
		{
			Assert.IsNotNullOrEmpty(value);

			return value;
		}

		public static string ShouldBeNull(this string value)
		{
			Assert.IsNull(value);

			return value;
		}

		public static string ShouldNotBeNull(this string value)
		{
			Assert.IsNotNull(value);

			return value;
		}

		public static string ShouldStartWith(this string value, string expected)
		{
			if (value == null && expected == null)
				return null;

			Assert.IsNotNull(value);
			Assert.IsTrue(value.StartsWith(expected), "Value should start with " + expected);

			return value;
		}

		public static string ShouldEndWith(this string value, string expected)
		{
			if (value == null && expected == null)
				return null;

			Assert.IsNotNull(value);
			Assert.IsTrue(value.EndsWith(expected), "Value should end with " + expected);

			return value;
		}

		public static string ShouldContain(this string value, string expected)
		{
			if (value == null && expected == null)
				return null;

			Assert.IsNotNull(value);
			Assert.IsTrue(value.Contains(expected), "Value should contain " + expected);

			return value;
		}
	}
}