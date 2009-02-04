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
    using Magnum.Serialization;
    using Machine.Specifications;
    using MbUnit.Framework;

    public class SecretObject
    {
        public SecretObject(string s)
        {
            StringValue = s;
        }

        public SecretObject()
        {
        }

        public string _hiddenValue;

        public string StringValue { get; private set; }
    }

    [Concern("SerializationUtil")]
    public class serializing_an_object_with_private_setters
    {
        private static readonly DateTime _dateTime = DateTime.Now;
        private static SecretObject _full;
        private static SecretObject _loaded;

        private Establish context = () => {_full = new SecretObject("Hello Johnson");};

        private Cleanup after_each = () =>
                                         {
                                             _full = null;
                                             _loaded = null;
                                         };

        private It should_properly_set_the_value_of_the_property = () => Assert.AreEqual(_full.StringValue, _loaded.StringValue);

        private Because the_object_has_been_transmogrified = () =>
                                                                 {
                                                                     byte[] intermediate = SerializationUtil<SecretObject>.SerializeToByteArray(_full);

                                                                     _loaded = SerializationUtil<SecretObject>.DeserializeFromByteArray(intermediate);
                                                                 };
    }
}