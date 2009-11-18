namespace Magnum.Specs.Cryptography
{
    using Magnum.Cryptography.PKI;
    using NUnit.Framework;

    [TestFixture]
    public class RsaCryptographyService_Specs
    {

        private const string PlainText = "chris is wacky";
        KeyPair _alice;
        KeyPair _bob;
        KeyPair _forEncryption;
        KeyPair _forDecryption;

        [SetUp]
        public void SetUp()
        {
            var pkg = new PublicKeyGenerator();
            _alice = pkg.MakeKeyPair();
            _bob = pkg.MakeKeyPair();

            //encrypting from bob -> alice
            _forEncryption = new KeyPair(_alice.Public, _bob.Private);
            _forDecryption = new KeyPair(_bob.Public, _alice.Private);
        }

        [Test]
        public void Smoke()
        {
            string result;

            using (IPkiCryptographyService srv = new RsaCryptographyService())
            {
                result = srv.Encrypt(_forEncryption, PlainText);
            }

            string decryptedText;

            using (IPkiCryptographyService srv2 = new RsaCryptographyService())
            {
                decryptedText = srv2.Decrypt(_forDecryption, result);
            }

            Assert.AreEqual(PlainText, decryptedText);
        }
    }
}