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
namespace Magnum.ProtocolBuffers.Specs.Serialization
{
    using System;
    using NUnit.Framework;
    using ProtocolBuffers.Serialization;

    [TestFixture]
    public class When_reading_from_a_CodedInputStream
    {
        [Test]
        public void An_Int32_should_be_returned()
        {
            byte[] input = new byte[] { 0x08, 0x96, 0x01 };
            var inputStream = new CodedInputStream(input);
            int value = 150;

            var msg = (int)inputStream.ReadNextMessage();

            Assert.AreEqual(value, msg); 
            
        }

        [Test]
        public void A_string_should_be_returned()
        {
            byte[] input = new byte[] { 0x12, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };
            var inputStream = new CodedInputStream(input);

            string value = "testing";

            var msg = (string)inputStream.ReadNextMessage();

            Assert.AreEqual(value, msg);   
        }
    }
}