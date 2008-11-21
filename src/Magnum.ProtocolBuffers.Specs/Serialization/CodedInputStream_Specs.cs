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
    using ProtocolBuffers.Serialization.Streams;

    [TestFixture]
    public class When_reading_from_a_CodedInputStream
    {
        [Test]
        public void Varint32()
        {
            byte[] input = new byte[] { 0x08, 0x96, 0x01 };
            var inputStream = new CodedInputStream(input);
            int value = 150;

            inputStream.ReadTag();
            var msg = inputStream.ReadVarint();

            Assert.AreEqual(value, msg); 
        }

        [Test]
        public void Varint64()
        {
            byte[] input = new byte[] {8,165,203,150,173,218,180,233,210,165,1 };
            var inputStream = new CodedInputStream(input);
            UInt64 value = 11936128518282651045;

            inputStream.ReadTag();
            var msg = inputStream.ReadVarint();

            Assert.AreEqual(value, msg);
        }

        [Test]
        public void A_fully_loaded_Uint64()
        {
            byte[] input = new byte[] { 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 };

            var inputStream = new CodedInputStream(input);
            
            const ulong value = 0xFFFFFFFFFFFFFFFFUL;

            inputStream.ReadTag();
            var msg = inputStream.ReadVarint();

            Assert.AreEqual(value, msg);
        }

        [Test]
        public void FixedInt32()
        {
            byte[] input = new byte[] { 0x15, 0x13, 0x00, 0x00, 0x0 };
            var inputStream = new CodedInputStream(input);
            int value = 19;

            inputStream.ReadTag();
            var msg = inputStream.ReadFixedInt32();

            Assert.AreEqual(value, msg); 
        }


        [Test]
        public void FixedInt64()
        {
            byte[] input = new byte[] { 0x09, 0x46, 0xE3, 0xB7, 0xD3, 0x9E, 0x74, 0x04, 0x00 };
            var inputStream = new CodedInputStream(input);
            Int64 value = 1254125412541254;

            inputStream.ReadTag();
            var msg = inputStream.ReadFixedInt64();

            Assert.AreEqual(value, msg); 
        }

        [Test]
        public void String()
        {
            byte[] input = new byte[] { 0x12, 0x07, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e, 0x67 };
            var inputStream = new CodedInputStream(input);

            string value = "testing";

            inputStream.ReadTag();
            var msg = inputStream.ReadString();

            Assert.AreEqual(value, msg);   
        }

        [Test]
        public void String_DiffLength()
        {
            byte[] input = new byte[] { 0x12, 0x06, 0x74, 0x65, 0x73, 0x74, 0x69, 0x6e };
            var inputStream = new CodedInputStream(input);

            string value = "testin";

            inputStream.ReadTag();
            var msg = inputStream.ReadString();

            Assert.AreEqual(value, msg);
        }
    }
}