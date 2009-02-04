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
    using Magnum.Cryptography;
    using MbUnit.Framework;

    [TestFixture]
    public class RijndaelCryptographyService_Specs
    {
        [Test]
        public void Smoke()
        {
            string plainText = "chris is wacky";
            string key = "eguhidbehumjdemy1234567890123456";
            Assert.AreEqual(32, key.Length);

            EncryptionResult result;

            using (ICryptographyService srv = new RijndaelCryptographyService(key))
            {
                result = srv.Encrypt(plainText);
            }

            string decryptedText;

            using (var srv2 = new RijndaelCryptographyService(key))
            {
                decryptedText = srv2.Decrypt(result);
            }

            Assert.AreEqual(plainText, decryptedText);
        }


        [Test]
        public void Do_Ivs_Have_the_same_length()
        {
            string key = "eguhidbehumjdemy";
            Assert.AreEqual(16, key.Length);

            var srv = new RijndaelCryptographyService(key);

            EncryptionResult result = srv.Encrypt("");
            EncryptionResult result2 = srv.Encrypt("2024");
            EncryptionResult result3 = srv.Encrypt("stnaoheutnahoetuhatnoeuhaonetuhnsaoteuh");

            string s = result2.Iv + result2.CipherText;

            Assert.AreEqual(32, result.Iv.Length);
            Assert.AreEqual(32, result2.Iv.Length);
            Assert.AreEqual(32, result3.Iv.Length);
        }
    }
}