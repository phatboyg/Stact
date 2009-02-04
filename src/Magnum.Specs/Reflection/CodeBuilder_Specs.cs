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
    using MbUnit.Framework;

    [TestFixture]
    public class When_building_a_delegate_using_lambdas :
        SpecificationBase
    {
        private ISerializationWriter _writer;
        private Customer _data;

        protected override void Before_each_specification()
        {
            _writer = Mock<ISerializationWriter>();

            _data = new Customer {FirstName = "Chris", LastName = "Patterson", Address = "123 American Way", Age = 27, Amount = 123.45m};
        }

        [Test]
        public void Verify_the_the_output_occurrs_in_order_and_properly()
        {
            using (Record)
            {
                _writer.Write("Chris");
                _writer.Write("Patterson");
                _writer.Write("123 American Way");
                _writer.Write(27);
                _writer.Write(123.45m);
            }

            using (Playback)
            {
                SerializationUtil<Customer>.Serialize(_writer, _data);
            }
        }

        [Test]
        public void I_want_to_write_fields_with_expression_trees()
        {
            Action<FieldClass, string> hitter = (x, y) => x._field = y;
    		
        }

        public class FieldClass
        {
            public string _field;
        }
    }
}