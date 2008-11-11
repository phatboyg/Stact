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

            var msg = (UInt64)inputStream.ReadNextMessage();

            Assert.AreEqual(value, msg); 
        }

        [Test]
        public void FixedInt32_Test()
        {
            byte[] input = new byte[] { 0x15, 0x13, 0x00, 0x00, 0x0 };
            var inputStream = new CodedInputStream(input);
            int value = 19;

            var msg = (Int32)inputStream.ReadNextMessage();

            Assert.AreEqual(value, msg); 
        }

        [Test]
        public void FixedInt64_Test()
        {
            byte[] input = new byte[] { 0x09, 0x46, 0xE3, 0xB7, 0xD3, 0x9E, 0x74, 0x04, 0x00 };
            var inputStream = new CodedInputStream(input);
            Int64 value = 1254125412541254;

            var msg = (Int64)inputStream.ReadNextMessage();

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

        [Test]
        public void Learning()
        {
            //1010 1100 0000 0010
            byte least = 0xAC;   //1010 1100
            byte most = 0x02; //0000 0010

            //strip MSB
            byte smallest = least.RemoveMsb(); //0010 1100
            byte biggest = most.RemoveMsb();   //0000 0010

            //combine and reverse
            byte[] b3 = new byte[2];
            b3[0] = (byte)((biggest << 7) | smallest);
            b3[1] = (byte)(biggest >>1);
            uint shifted = (uint)(b3[1] << 8);
            Assert.AreEqual(300, shifted + b3[0]);


            byte ae = 0x40; //0100 0000
            byte ac = 0x01; //0000 0001
            //i want to get one byte 1100 0000
            ae = (byte)((ac << 7) | ae);
            Assert.AreEqual(0xC0, ae);
        }
    }
}