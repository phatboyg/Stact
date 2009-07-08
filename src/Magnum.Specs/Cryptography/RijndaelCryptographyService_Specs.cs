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
namespace Magnum.Specs.Cryptography
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Magnum.Cryptography;
    using MbUnit.Framework;

    [TestFixture]
    public class RijndaelCryptographyService_Specs
    {
        private const string PlainText = "chris is wacky";
        private const string Key = "eguhidbehumjdemy1234567890123456";
        private byte[] _clearBytes;
        private Stream _clearStream;

        [Test]
        public void Smoke()
        {
            EncryptedText result;

            using (ICryptographyService srv = new RijndaelCryptographyService(Key))
            {
                result = srv.Encrypt(PlainText);
            }

            string decryptedText;

            using (ICryptographyService srv2 = new RijndaelCryptographyService(Key))
            {
                decryptedText = srv2.Decrypt(result);
            }

            Assert.AreEqual(PlainText, decryptedText);
        }

        [Test]
        public void Smoke_stream()
        {
            _clearBytes = Encoding.UTF8.GetBytes(PlainText);
            _clearStream = new MemoryStream(_clearBytes);

            EncryptedStream encryptionResult;

            using (ICryptographyService srv = new RijndaelCryptographyService(Key))
            {
                encryptionResult = srv.Encrypt(_clearStream);
            }

            _clearStream.Position = 0;
            Stream decryptedStream;

            using (ICryptographyService srv2 = new RijndaelCryptographyService(Key))
            {
                decryptedStream = srv2.Decrypt(encryptionResult);
            }

            AssertStreams(_clearStream, decryptedStream);
        }

        [Test]
        public void Smoke_stream2()
        {
            var tryThese = new List<string>
                {
                    "hear back from alumni and be informed as to how they are progressing in both their personal and professional lives. To feel like you have had a positive impact and were able to add value to someone's life is an awesome thing. I just received an email from a recent alumni who signed his email with the following:",
                    @"<?xml version=""1.0"" encoding=""utf-8""?>
<x:XmlMessageEnvelope xmlns:x=""MassTransit.Serialization.XmlMessageEnvelope, MassTransit, Version=0.5.0.1927, Culture=neutral, PublicKeyToken=null"" xmlns:m=""MassTransit.Serialization.EncryptedMessageEnvelope, MassTransit, Version=0.5.0.1927, Culture=neutral, PublicKeyToken=null"" xmlns:s=""System.String, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" xmlns:i=""System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"">
  <m:Message>
    <s:CipheredMessage>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</s:CipheredMessage>
    <s:IV>5HeLBvGhHZ3ox8QSBJJ2/QrcPUL+s872LZfEp/Lofn4=</s:IV>
  </m:Message>
  <i:RetryCount>0</i:RetryCount>
  <s:MessageType>MassTransit.Serialization.EncryptedMessageEnvelope, MassTransit</s:MessageType>
</x:XmlMessageEnvelope>",
                    new string('z', 4000)
                };

            foreach (var thing in tryThese)
            {
                _clearBytes = Encoding.UTF8.GetBytes(thing);
                var plainStrea = new MemoryStream(_clearBytes);

                EncryptedStream result;

                using (ICryptographyService srv = new RijndaelCryptographyService(Key))
                {
                    result = srv.Encrypt(plainStrea);
                }

                plainStrea.Position = 0;
                Stream decryptedStream;

                using (ICryptographyService srv2 = new RijndaelCryptographyService(Key))
                {
                    decryptedStream = srv2.Decrypt(result);
                }

                AssertStreams(plainStrea, decryptedStream);
            }
        }

        private void AssertStreams(Stream expected, Stream actual)
        {
            for (int i = 0; i < expected.Length; i++)
            {
                var e = expected.ReadByte();
                var a = actual.ReadByte();

                Assert.AreEqual(e, a);
            }
        }

        [Test]
        public void Do_Ivs_Have_the_same_length()
        {
            string key = "eguhidbehumjdemy";
            Assert.AreEqual(16, key.Length);

            var srv = new RijndaelCryptographyService(key);

            EncryptedText result = srv.Encrypt("");
            EncryptedText result2 = srv.Encrypt("2024");
            EncryptedText result3 = srv.Encrypt("stnaoheutnahoetuhatnoeuhaonetuhnsaoteuh");

            string s = result2.Iv + result2.CipherText;

            Assert.AreEqual(32, result.Iv.Length);
            Assert.AreEqual(32, result2.Iv.Length);
            Assert.AreEqual(32, result3.Iv.Length);
        }
    }
}