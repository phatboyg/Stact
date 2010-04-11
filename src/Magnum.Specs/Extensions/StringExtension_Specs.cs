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
namespace Magnum.Specs.Extensions
{
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Checking_if_a_string_is_empty
	{
		[Test]
		public void Should_return_true_if_the_string_is_null()
		{
			string value = null;

			value.IsEmpty().ShouldBeTrue();
		}

		[Test]
		public void Should_return_true_if_the_string_has_no_characters()
		{
			string value = "";

			value.IsEmpty().ShouldBeTrue();
		}

		[Test]
		public void Should_return_false_if_the_string_has_a_value()
		{
			string value = "A";

			value.IsEmpty().ShouldBeFalse();
		}
	}

	[TestFixture]
	public class Checking_if_a_string_is_not_empty
	{

		[Test]
		public void Should_return_true_if_the_string_has_a_value()
		{
			string value = "A";

			value.IsNotEmpty().ShouldBeTrue();
		}

		[Test]
		public void Should_return_false_if_the_string_is_null()
		{
			string value = null;

			value.IsNotEmpty().ShouldBeFalse();
		}

		[Test]
		public void Should_return_false_if_the_string_has_no_characters()
		{
			string value = "";

			value.IsNotEmpty().ShouldBeFalse();
		}
	}

	[TestFixture]
	public class Checking_if_a_string_is_null
	{
		[Test]
		public void Should_return_true_if_the_string_is_null()
		{
			string value = null;

			value.IsNull().ShouldBeTrue();
		}

		[Test]
		public void Should_return_false_if_the_string_is_not_null()
		{
			string value = "";

			value.IsNull().ShouldBeFalse();
		}
	}

	
}