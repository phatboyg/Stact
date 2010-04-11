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
namespace Magnum.Web.Specs
{
	using System;
	using System.Collections.Generic;
	using Binding;
	using NUnit.Framework;
	using TestFramework;

	[TestFixture]
	public class Binding_a_dictionary_to_an_object
	{
		private Dictionary<string, object> _dictionary;
		private BinderTestClass _result;

		[TestFixtureSetUp]
		public void Setup()
		{
			_dictionary = new Dictionary<string, object>
				{
					{"StringValue", "Value"},
					{"IntValue", "47"},
					{"Created", "2010-03-01 12:34:56.123"},
					{"Duration", "12:34:56.123"},
					{"SubClass_Street", "123 American Way"},
					{"SubClass_City", "Tulsa"},
					{"SubClass_State", "OK"},
				};

			ModelBinder binder = new FastModelBinder();
			ModelBinderContext context = new TestModelBinderContext(_dictionary);

			object obj = binder.Bind(typeof (BinderTestClass), context);

			_result = obj as BinderTestClass;
		}

		private class BinderTestClass
		{
			public string StringValue { get; set; }
			public int IntValue { get; set; }
			public DateTime Created { get; set; }
			public TimeSpan Duration { get; set; }
			public BinderSubClass SubClass { get; set; }
		}

		private class BinderSubClass
		{
			public string Street { get; set; }
			public string City { get; set; }
			public string State { get; set; }
		}

		[Test]
		public void Should_bind_the_subclass_city()
		{
			_result.SubClass.City.ShouldEqual("Tulsa");
		}

		[Test]
		public void Should_bind_the_subclass_state()
		{
			_result.SubClass.State.ShouldEqual("OK");
		}

		[Test]
		public void Should_bind_the_subclass_string()
		{
			_result.SubClass.Street.ShouldEqual("123 American Way");
		}

		[Test]
		public void Should_not_return_a_null_object()
		{
			_result.ShouldNotBeNull();
		}

		[Test]
		public void Should_properly_bind_a_integer_value()
		{
			_result.IntValue.ShouldEqual(47);
		}

		[Test]
		public void Should_properly_bind_a_string_value()
		{
			_result.StringValue.ShouldEqual("Value");
		}
		[Test]
		public void Should_bind_datetime()
		{
			_result.Created.ShouldEqual(new DateTime(2010, 3, 1, 12, 34, 56, 123));
		}

		[Test]
		public void Should_bind_timespan()
		{
			_result.Duration.ShouldEqual(new TimeSpan(0, 12, 34, 56, 123));
		}

		[Test]
		public void Should_property_bind_the_subclass_property()
		{
			_result.SubClass.ShouldNotBeNull();
		}
	}
}