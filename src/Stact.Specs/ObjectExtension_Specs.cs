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
namespace Stact.Specs
{
	using System;
	using System.Collections.Generic;
	using Stact.Extensions;
	using NUnit.Framework;

	[TestFixture]
	public class When_a_value_is_being_verified
	{
		[Test]
		public void An_exception_should_be_thrown_if_the_value_is_out_of_range()
		{
			var items = new List<int> {1, 2, 3, 4, 5, 6, 7};
			int value = 27;

			try
			{
				value.MustBeInRange(0.Through(items.Count));

				Assert.Fail("The exception should have been thrown");
			}
			catch (ArgumentException)
			{
			}
			catch (Exception)
			{
				Assert.Fail("Should have been an ArgumentException");
			}
		}

		[Test]
		public void TestFormatWith()
		{
			Assert.AreEqual("'rob'", "'{0}'".FormatWith("rob"));
		}
	}
}