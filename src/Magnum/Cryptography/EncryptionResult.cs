namespace Magnum.Cryptography
{
    using System.IO;

    public class EncryptionResult
    {
        public EncryptionResult(string cipherText, byte[] iv)
        {
            CipherText = cipherText;
            Iv = iv;
        }

        public string CipherText { get; private set; }
        public byte[] Iv { get; private set; }
    }

    public class EncryptionStreamResult
    {
        public EncryptionStreamResult(Stream cipherStream, byte[] iv)
        {
            CipherStream = cipherStream;
            Iv = iv;
        }

        public Stream CipherStream { get; private set; }
        public byte[] Iv { get; private set; }
    }
}