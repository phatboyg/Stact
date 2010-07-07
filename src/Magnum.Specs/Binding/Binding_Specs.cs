namespace Magnum.Specs.Binding
{
	using System;
	using System.Collections.Generic;
	using Magnum.Binding;
	using Magnum.ValueProviders;
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
					{"SubClass.Street", "123 American Way"},
					{"SubClass.City", "Tulsa"},
					{"SubClass.State", "OK"},
                    //no enum value provided on purpose
				};

			ValueProvider dictionaryProvider = new DictionaryValueProvider(_dictionary);
			ValueProvider jsonProvider = new JsonValueProvider(@"{ SubClass: { ZipCode: ""90210"" } }");

			var providers = new MultipleValueProvider(new[] { dictionaryProvider, jsonProvider });

			ModelBinder binder = new FastModelBinder();
			ModelBinderContext context = new TestModelBinderContext(providers);

			object obj = binder.Bind(typeof(BinderTestClass), context);

			_result = obj as BinderTestClass;
		}

		private class BinderTestClass
		{
			public string StringValue { get; set; }
			public int IntValue { get; set; }
			public DateTime Created { get; set; }
			public TimeSpan Duration { get; set; }
			public BinderSubClass SubClass { get; set; }
			public BinderEnum AnEnum { get; set; }
		}

		private class BinderSubClass
		{
			public string Street { get; set; }
			public string City { get; set; }
			public string State { get; set; }
			public string ZipCode { get; set; }
		}

		private enum BinderEnum
		{
			DefaultValue,
			Value1,
			Value2
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
		public void Should_bind_the_subclass_zipcode()
		{
			_result.SubClass.ZipCode.ShouldEqual("90210");
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

		[Test]
		public void Should_use_the_first_enum_entry_when_not_provided()
		{
			_result.AnEnum.ShouldEqual(BinderEnum.DefaultValue);
		}
	}
}
