namespace Magnum.Specs.Serialization
{
	using System;
	using TestFramework;

	[Scenario]
	public class Given_an_interface_with_a_property_of_type<T> :
		With_the_fast_text_serializer
	{
		private readonly string _expected;
		private readonly T _value;
		private string _body;
		private TargetClass<T> _target;

		protected Given_an_interface_with_a_property_of_type(T value, string expected)
		{
			_value = value;
			_expected = expected;
		}

		[Given]
		public void A_property_on_an_object_of_type()
		{
			_target = new TargetClass<T>(_value);
		}

		[When]
		public void An_object_is_serialized()
		{
			_body = Subject.Serialize(_target);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual(_expected);
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<TargetInterface<T>>(_body);

			value.ShouldNotBeNull();

			value.Value.ShouldEqual(_value);
		}

		public interface TargetInterface<TValue>
		{
			TValue Value { get; }	
		}

		private class TargetClass<TValue> :
			TargetInterface<TValue>
		{
			protected TargetClass()
			{
			}

			public TargetClass(TValue value)
			{
				Value = value;
			}

			public TValue Value { get; set; }
		}
	}

	[Scenario]
	public class Serializing_a_date_time_on_an_interface :
		Given_an_interface_with_a_property_of_type<DateTime>
	{
		public Serializing_a_date_time_on_an_interface()
			: base(new DateTime(2010, 3, 4, 12, 34, 56, 123, DateTimeKind.Utc), "{Value:2010-03-04T12:34:56.123Z}")
		{
		}
	}
}
