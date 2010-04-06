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
namespace Magnum.Specs
{
	using System;
	using NUnit.Framework;

	[TestFixture]
	public class Passing_a_null_value_to_a_method
	{
		private class Subject
		{
			public void TargetMethod(string value)
			{
				Guard.AgainstNull(value);
			}

			public void GreaterThanMethod(int value)
			{
				Guard.GreaterThan(27, value);
			}

			public void LessThanMethod(int value)
			{
				Guard.LessThan(100, value);
			}
		}

		[Test]
		public void Should_evaluate_the_delegate_and_throw_if_not_true()
		{
			Assert.Throws<ArgumentException>(() => Guard.IsTrue(x => x.Length > 0, ""));
		}

		[Test]
		public void Should_throw_an_argument_if_the_value_is_equal()
		{
			var subject = new Subject();

			Assert.Throws<ArgumentOutOfRangeException>(() => subject.GreaterThanMethod(27));
		}

		[Test]
		public void Should_throw_an_argument_if_the_value_is_too_low()
		{
			var subject = new Subject();

			Assert.Throws<ArgumentOutOfRangeException>(() => subject.GreaterThanMethod(26));
		}

		[Test]
		public void Should_throw_an_argument_if_value_is_too_high()
		{
			var subject = new Subject();

			Assert.Throws<ArgumentOutOfRangeException>(() => subject.LessThanMethod(101));
		}

		[Test]
		public void Should_throw_an_argument_null_exception()
		{
			var subject = new Subject();

			Assert.Throws<ArgumentNullException>(() => subject.TargetMethod(null));
		}
	}
}