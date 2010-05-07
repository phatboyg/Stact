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
	using Magnum.Serialization;
	using TestFramework;

	[Scenario]
	public class With_the_fast_text_serializer
	{
		protected Serializer Subject { get; set; }

		[Given]
		public void Given_the_fast_text_serializer()
		{
			Subject = new FastTextSerializer();
		}
	}


	[Scenario]
	public class When_serializing_a_boolean_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private bool _value;

		[When]
		public void A_boolean_is_serialized()
		{
			_value = true;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("true");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<bool>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_byte_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private byte _value;

		[When]
		public void A_byte_is_serialized()
		{
			_value = 47;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("47");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<byte>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_char_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private char _value;

		[When]
		public void A_char_is_serialized()
		{
			_value = 'A';
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("A");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<char>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_datetimeoffset_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private DateTimeOffset _value;

		[When]
		public void A_datetimeoffset_is_serialized()
		{
			_value = new DateTimeOffset(2010, 03, 27, 12, 34, 56, 147, -6.Hours());
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("2010-03-27T18:34:56.147Z");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<DateTimeOffset>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_datetime_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private DateTime _value;

		[When]
		public void A_datetime_is_serialized()
		{
			_value = new DateTime(2010, 03, 27, 12, 34, 56, 147, DateTimeKind.Utc);
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("2010-03-27T12:34:56.147Z");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<DateTime>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_decimal_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private decimal _value;

		[When]
		public void A_decimal_is_serialized()
		{
			_value = -123.4567m;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("-123.4567");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<decimal>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_double_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private double _value;

		[When]
		public void A_double_is_serialized()
		{
			_value = -123.4567d;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("-123.4567");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<double>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_enum_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private SomeEnum _value;

		private enum SomeEnum
		{
			Default = 0,
			Superior = 47,
		}

		[When]
		public void An_enum_is_serialized()
		{
			_value = SomeEnum.Superior;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("Superior");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<SomeEnum>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_float_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private float _value;

		[When]
		public void A_float_is_serialized()
		{
			_value = -123.456f;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("-123.456");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<float>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_guid_value :
		With_the_fast_text_serializer
	{
		private const string _expected = "e6975bb8789940b2a8be117d74c12877";
		private string _body;
		private Guid _value;

		[When]
		public void A_guid_is_serialized()
		{
			_value = new Guid(_expected);

			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual(_expected);
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<Guid>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_int_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private int _value;

		[When]
		public void A_int_is_serialized()
		{
			_value = -90210;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("-90210");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<int>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_long_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private long _value;

		[When]
		public void A_long_is_serialized()
		{
			_value = 91855512122112;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("91855512122112");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<long>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_sbyte_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private sbyte _value;

		[When]
		public void A_sbyte_is_serialized()
		{
			_value = 127;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("127");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<sbyte>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_short_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private short _value;

		[When]
		public void A_short_is_serialized()
		{
			_value = 32767;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("32767");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<short>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_timespan_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private TimeSpan _value;

		[When]
		public void A_short_is_serialized()
		{
			_value = new TimeSpan(40, 1, 2, 3, 4);
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("P40DT1H2M3.004S");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<TimeSpan>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_uint_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private uint _value;

		[When]
		public void A_uint_is_serialized()
		{
			_value = 4000000000u;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("4000000000");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<uint>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_ulong_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private ulong _value;

		[When]
		public void A_uint_is_serialized()
		{
			_value = 4000000000000000ul;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("4000000000000000");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<ulong>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_uri_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private Uri _value;
		private string _expected;

		[When]
		public void A_ushort_is_serialized()
		{
			_expected = "http://github.com/phatboyg/Magnum";
			_value = new Uri(_expected);
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual(_expected);
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<Uri>(_body);

			value.ShouldEqual(_value);
		}
	}

	[Scenario]
	public class When_serializing_a_ushort_value :
		With_the_fast_text_serializer
	{
		private string _body;
		private ushort _value;

		[When]
		public void A_ushort_is_serialized()
		{
			_value = 65535;
			_body = Subject.Serialize(_value);
		}

		[Then]
		public void Should_create_the_proper_serialized_body()
		{
			_body.ShouldEqual("65535");
		}

		[Then]
		public void Should_deserialize_to_the_proper_value()
		{
			var value = Subject.Deserialize<ushort>(_body);

			value.ShouldEqual(_value);
		}
	}
}