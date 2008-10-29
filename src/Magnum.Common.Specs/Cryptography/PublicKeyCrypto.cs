namespace Magnum.Common.Specs.Cryptography
{
    using System;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using NUnit.Framework;

    [TestFixture]
    public class PublicKeyCrypto
    {
        //http://www.source-code.biz/snippets/vbasic/3.htm
        //VISTA: http://windowshelp.microsoft.com/Windows/en-US/Help/a57a997d-cad4-4b95-84b1-efb3ede7cd521033.mspx

        [Test]
        public void Test()
        {
            X509Certificate2 aliceCert = new X509Certificate2(".\\alice.pfx", "1234"); //password: 1234
            X509Certificate2 bobCert = new X509Certificate2(".\\bob.pfx", "1234"); //password: 1234

            string message = "Meet me at noon";

            var encryptor = (RSACryptoServiceProvider)aliceCert.PublicKey.Key;
            var plainBytes = Convert.FromBase64String(message);
            var cryptBytes = encryptor.Encrypt(plainBytes, false);
            
            var cryptString = Convert.ToBase64String(cryptBytes);
            Trace.Write(cryptString);

            //decrypt with private key
            var decryptor = (RSACryptoServiceProvider) aliceCert.PrivateKey;
            var decryptBytes = decryptor.Decrypt(cryptBytes, false); //TODO: Error Here
            var decryptString = Convert.ToBase64String(decryptBytes);
            
            
            Assert.AreEqual(message, decryptString);
        }
    }
}