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
namespace Stact.Specs.Functional
{
	/*
	using Magnum.TestFramework;

	[Scenario]
	public class Maybe_monad
	{
		[Then]
		public void Should_assign_properly()
		{
			string subject = "coffee".ToMaybe()
				.SelectMany(x => x + " tastes good")
				.SelectMany(x => x + " with cream")
				.Value;

			subject.ShouldEqual("coffee tastes good with cream");
		}

		[Then]
		public void Should_chain()
		{
			var r = 5.ToIdentity().SelectMany(x => 6.ToIdentity(), (x, y) => x + y);

			r.Value.ShouldEqual(11);
		}

		[Then]
		public void Should_linq()
		{
			var t = from x in 5.ToIdentity()
					from y in 6.ToIdentity()
					select x + y;

			t.Value.ShouldEqual(11);
		}

		[Then]
		public void Should_get_angry_with_null_values()
		{
			string subject = "coffee".ToMaybe()
				.SelectMany(x => (string)null)
				.SelectMany(x => x + " with cream")
				.Value;

			subject.ShouldBeNull();
		}

		[Then]
		public void Should_support_select()
		{
			var subject = from s in 5.ToMaybe()
			                 from a1 in 6.ToMaybe()
			                 select s + a1;

			subject.Value.ShouldEqual(11);
		}

		[Then]
		public void Should_support_strings()
		{
			var subject = from left in "left".ToMaybe()
			              from right in "right".ToMaybe()
			              select left + right;

			subject.Value.ShouldEqual("leftright");
		}

		[Then]
		public void Should_chain_multiple_types()
		{
			var subject = from left in "left".ToMaybe()
			              from right in "right".ToIdentity()
			              select left + right;

			subject.Value.ShouldEqual("leftright");
		}

		[Then]
		public void Should_chain_no_value_maybes()
		{
			var subject = from left in Maybe<string>.Nothing
			              from right in "right".ToMaybe()
			              select left + right;

			subject.Value.ShouldBeNull();
		}
	}*/
}