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
namespace Magnum.Specs.Reflection
{
    using System;
    using Common.Serialization;
    using Machine.Specifications;
    using MbUnit.Framework;

    [Concern("SerializationUtil")]
    public class serializing_and_deserializing_an_object
    {
        private static readonly DateTime _dateTime = DateTime.Now;
        private static FullObjectType _full;
        private static FullObjectType _loaded;

        private Cleanup after_each = () => {_full = null;};

        private Establish context = () =>
                                        {
                                            _full = new FullObjectType
                                                        {
                                                            BooleanValue = true,
                                                            DateTimeValue = _dateTime,
                                                            DecimalValue = 123.45m,
                                                            DoubleValue = 123.45d,
                                                            FloatValue = 1234.56f,
                                                            IntValue = 123,
                                                            LongValue = 123456789,
                                                            ShortValue = 1235,
                                                            StringValue = "Hello Johnson",
                                                            UIntValue = 89123U,
                                                            ULongValue = 891823UL,
                                                            UShortValue = 1231,
                                                        };
                                        };

        private Because the_object_has_been_transmogrified = () =>
                                                                 {
                                                                     byte[] intermediate = SerializationUtil<FullObjectType>.SerializeToByteArray(_full);

                                                                     _loaded = SerializationUtil<FullObjectType>.DeserializeFromByteArray(intermediate);
                                                                 };

        private It should_serialize_a_boolean_properly = () => Assert.AreEqual(_full.BooleanValue, _loaded.BooleanValue);
        private It should_serialize_a_DateTime_properly = () => Assert.AreEqual(_full.DateTimeValue, _loaded.DateTimeValue);
        private It should_serialize_a_decimal_properly = () => Assert.AreEqual(_full.DecimalValue, _loaded.DecimalValue);
        private It should_serialize_a_double_properly = () => Assert.AreEqual(_full.DoubleValue, _loaded.DoubleValue);
        private It should_serialize_a_single_properly = () => Assert.AreEqual(_full.FloatValue, _loaded.FloatValue);
        private It should_serialize_an_int_properly = () => Assert.AreEqual(_full.IntValue, _loaded.IntValue);
        private It should_serialize_a_long_properly = () => Assert.AreEqual(_full.LongValue, _loaded.LongValue);
        private It should_serialize_a_short_properly = () => Assert.AreEqual(_full.ShortValue, _loaded.ShortValue);
        private It should_serialize_a_string_properly = () => Assert.AreEqual(_full.StringValue, _loaded.StringValue);
        private It should_serialize_an_unsigned_int_properly = () => Assert.AreEqual(_full.UIntValue, _loaded.UIntValue);
        private It should_serialize_an_unsigned_long_properly = () => Assert.AreEqual(_full.ULongValue, _loaded.ULongValue);
        private It should_serialize_an_unsigned_short_properly = () => Assert.AreEqual(_full.UShortValue, _loaded.UShortValue);
    }

    public class FullObjectType
    {
        public bool BooleanValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public decimal DecimalValue { get; set; }
        public double DoubleValue { get; set; }
        public float FloatValue { get; set; }
        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public short ShortValue { get; set; }
        public string StringValue { get; set; }
        public uint UIntValue { get; set; }
        public ulong ULongValue { get; set; }
        public ushort UShortValue { get; set; }
    }
}