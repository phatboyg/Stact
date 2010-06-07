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
namespace Magnum.Specs.Monads
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using Magnum.Reflection;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Settings_a_property_on_a_missing_member
	{
		[Test]
		public void Should_allow_property_to_already_be_set()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inner.Value;

			SafeProperty<OuterClass> writer = accessor.CreateSafeProperty();

			const string expected = "Hello";

			var subject = new OuterClass();
			subject.Inner = new InnerClass {OtherValue = "Hi"};

			writer.Set(subject, expected);

			subject.Inner.Value.ShouldEqual(expected);
			subject.Inner.OtherValue.ShouldEqual("Hi");
		}

		[Test]
		public void Should_create_a_setter_for_a_simple_value()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Value;


			SafeProperty<OuterClass> writer = accessor.CreateSafeProperty();


			var subject = new OuterClass();

			const string expected = "Hello";

			writer.Set(subject, expected);

			subject.Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_go_way_deep()
		{
			Expression<Func<WayOuterClass, object>> accessor = o => o.Outer.Inner.Value;

			SafeProperty<WayOuterClass> writer = accessor.CreateSafeProperty();

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer.Set(subject, expected);

			subject.Outer.Inner.Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_not_throw_a_null_reference_exception()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inner.Value;

			SafeProperty<OuterClass> writer = accessor.CreateSafeProperty();

			const string expected = "Hello";

			var subject = new OuterClass();

			writer.Set(subject, expected);

			subject.Inner.Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_create_a_backing_list_for_list_based_properties()
		{
			Expression<Func<OuterClass, object>> accessor = o => o.Inners[0].Value;

			SafeProperty<OuterClass> writer = accessor.CreateSafeProperty();

			const string expected = "Hello";

			var subject = new OuterClass();

			writer.Set(subject, expected);

			subject.Inners.ShouldNotBeNull();
			subject.Inners[0].Value.ShouldEqual(expected);
		}

		[Test]
		public void Should_create_a_backing_list_for_list_based_properties_way_deep()
		{
			Expression<Func<WayOuterClass, object>> accessor = o => o.Outer.Inners[0].Value;

			SafeProperty<WayOuterClass> writer = accessor.CreateSafeProperty();

			const string expected = "Hello";

			var subject = new WayOuterClass();

			writer.Set(subject, expected);

			subject.Outer.Inners.ShouldNotBeNull();
			subject.Outer.Inners[0].Value.ShouldEqual(expected);
		}

		public class InnerClass
		{
			public string Value { get; set; }
			public string OtherValue { get; set; }
		}

		public class OuterClass
		{
			public InnerClass Inner { get; set; }
			public string Value { get; set; }
			public IList<InnerClass> Inners { get; set; }
		}

		public class WayOuterClass
		{
			public OuterClass Outer { get; private set; }
		}
	}
}