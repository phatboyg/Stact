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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;

	public static class AssertionsForCollections
	{
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

		public static IEnumerable<T> ShouldEqual<T>(this IEnumerable<T> values, IEnumerable<T> expected)
		{
			Assert.IsNotNull(values, "Target enumeration cannot be null");
			Assert.IsNotNull(expected, "Expected enumeration cannot be null");

			var extraValues = new List<T>();
			var missingValues = new List<T>();

			using (IEnumerator<T> e1 = values.GetEnumerator())
			using (IEnumerator<T> e2 = expected.GetEnumerator())
			{
				while (e1.MoveNext())
				{
					if (!e2.MoveNext())
					{
						extraValues.Add(e1.Current);
					}
					else
					{
						Assert.AreEqual(e2.Current, e1.Current);
					}
				}
				while (e2.MoveNext())
				{
					missingValues.Add(e2.Current);
				}
			}

			if (extraValues.Count > 0)
			{
				Assert.Fail("The target enumeration contained too many values: " + string.Join(", ", extraValues.Select(x => x.ToString()).ToArray()));
			}

			if (missingValues.Count > 0)
			{
				Assert.Fail("The target enumeration was missing values: " + string.Join(", ", missingValues.Select(x => x.ToString()).ToArray()));
			}

			return values;
		}
	}
}