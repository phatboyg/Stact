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
namespace Magnum.Specs.Serialization
{
	using System;
	using Magnum.Extensions;
	using NUnit.Framework;
	using TestFramework;

	[Scenario]
	public class Given_a_property_on_an_object_of_type<T> :
		With_the_fast_text_serializer
	{
		private readonly string _expected;
		private readonly T _value;
		private string _body;
		private TargetClass<T> _target;

		protected Given_a_property_on_an_object_of_type(T value, string expected)
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
			var value = Subject.Deserialize<TargetClass<T>>(_body);

			value.ShouldNotBeNull();

			value.Value.ShouldEqual(_value);
		}

		private class TargetClass<TValue>
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
	public class Serializing_an_object_with_lots_of_properties :
		With_the_fast_text_serializer
	{
		private string _body;
		private TargetClass _value;

		[Given]
		public void A_fully_loaded_object()
		{
			
			_value = new TargetClass
				{
					Boolean = true,
					Byte = 127,
					Char = 'A',
					DateTime = new DateTime(2010, 12, 1, 15, 12, 19, 27, DateTimeKind.Utc),
					DateTimeOffset = new DateTimeOffset(2010, 4, 15, 12, 34, 56, 123, 0.Hours()),
					Decimal = 123.45m,
					Double = 123.45,
					Float = 123.45f,
					Guid = new Guid("71949725b986495db59bafc4e2d288b3"),
					Int = 47,
					Long = 8675309,
					String = "There was a time in 1942, an early 8:43 AM response, with 123.72 responses",
					TimeSpan = new TimeSpan(12, 4, 5, 6, 7),
					Uri = new Uri("http://www.google.com")
				};
		}

		[When]
		public void An_object_is_serialized()
		{
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("{Boolean:true,Byte:127,Char:A,DateTime:2010-12-01T15:12:19.027Z,DateTimeOffset:2010-04-15T12:34:56.123Z,Decimal:123.45,Double:123.45,Float:123.45,Guid:71949725b986495db59bafc4e2d288b3,Int:47,Long:8675309,String:\"There was a time in 1942, an early 8:43 AM response, with 123.72 responses\",TimeSpan:P12DT4H5M6.007S,Uri:http://www.google.com/}");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<TargetClass>(_body);

			value.ShouldNotBeNull();

			value.ShouldEqual(_value);
		}

		private class TargetClass
		{
			public bool Boolean { get; set; }
			public byte Byte { get; set; }
			public char Char { get; set; }
			public DateTime DateTime { get; set; }
			public DateTimeOffset DateTimeOffset { get; set; }
			public decimal Decimal { get; set; }
			public double Double { get; set; }
			public float Float { get; set; }
			public Guid Guid { get; set; }
			public int Int { get; set; }
			public long Long { get; set; }
			public string String { get; set; }
			public TimeSpan TimeSpan { get; set; }
			public Uri Uri { get; set; }

			public bool Equals(TargetClass other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(this, other)) return true;
				return other.Boolean.Equals(Boolean) && other.Byte == Byte && other.Char == Char && other.DateTime.Equals(DateTime)
				       && other.DateTimeOffset.Equals(DateTimeOffset) && other.Decimal == Decimal && other.Double == Double
				       && other.Float == Float && other.Int == Int && other.Guid.Equals(Guid) && other.Long == Long
				       && Equals(other.String, String) && other.TimeSpan.Equals(TimeSpan) && Equals(other.Uri, Uri);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (TargetClass)) return false;
				return Equals((TargetClass) obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					int result = Boolean.GetHashCode();
					result = (result*397) ^ Byte.GetHashCode();
					result = (result*397) ^ Char.GetHashCode();
					result = (result*397) ^ DateTime.GetHashCode();
					result = (result*397) ^ DateTimeOffset.GetHashCode();
					result = (result*397) ^ Decimal.GetHashCode();
					result = (result*397) ^ Double.GetHashCode();
					result = (result*397) ^ Float.GetHashCode();
					result = (result*397) ^ Int;
					result = (result*397) ^ Guid.GetHashCode();
					result = (result*397) ^ Long.GetHashCode();
					result = (result*397) ^ (String != null ? String.GetHashCode() : 0);
					result = (result*397) ^ TimeSpan.GetHashCode();
					result = (result*397) ^ (Uri != null ? Uri.GetHashCode() : 0);
					return result;
				}
			}
		}
	}

    [Scenario, Explicit("Not yet supported.")]
    public class Serializing_an_type_object_property :
        Given_a_property_on_an_object_of_type<Type>
    {
        public Serializing_an_type_object_property() :
            //base(typeof(string), "{Value:\"System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"}")
            base(typeof(string), "{Value:\"System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"}")
        {
        }
    }

	[Scenario]
	public class Serializing_an_int_object_property :
		Given_a_property_on_an_object_of_type<int>
	{
		public Serializing_an_int_object_property()
			: base(27, "{Value:27}")
		{
		}
	}

	[Scenario]
	public class Serializing_a_nullable_int_object_property :
		Given_a_property_on_an_object_of_type<int?>
	{
		public Serializing_a_nullable_int_object_property()
			: base(27, "{Value:27}")
		{
		}
	}

	[Scenario]
	public class Serializing_a_string_object_property :
		Given_a_property_on_an_object_of_type<string>
	{
		public Serializing_a_string_object_property()
			: base("Alpha", "{Value:Alpha}")
		{
		}
	}

	[Scenario]
	public class Serializing_a_string_object_property_with_special_characters :
		Given_a_property_on_an_object_of_type<string>
	{
		public Serializing_a_string_object_property_with_special_characters()
			: base("Jackson, Andrew (1:2)", "{Value:\"Jackson, Andrew (1:2)\"}")
		{
		}
	}

	[Scenario]
	public class Serializing_a_date_time_object_property :
		Given_a_property_on_an_object_of_type<DateTime>
	{
		public Serializing_a_date_time_object_property()
			: base(new DateTime(2010, 3, 4, 12, 34, 56, 123, DateTimeKind.Utc), "{Value:2010-03-04T12:34:56.123Z}")
		{
		}
	}
}