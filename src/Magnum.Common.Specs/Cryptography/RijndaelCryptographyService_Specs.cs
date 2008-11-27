using Magnum.Common.Cryptography;
using MbUnit.Framework;

namespace Magnum.Common.Specs.Cryptography
{
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
        


        [Test()]
        public void Do_Ivs_Have_the_same_length()
        {

            string key = "eguhidbehumjdemy";
            Assert.AreEqual(16, key.Length);

            var srv = new RijndaelCryptographyService(key);

            var result = srv.Encrypt("");
            var result2 = srv.Encrypt("2024");
            var result3 = srv.Encrypt("stnaoheutnahoetuhatnoeuhaonetuhnsaoteuh");

            string s = result2.Iv.ToString() + result2.CipherText;

            Assert.AreEqual(32, result.Iv.Length);
            Assert.AreEqual(32, result2.Iv.Length);
            Assert.AreEqual(32, result3.Iv.Length);

        }
    }
}