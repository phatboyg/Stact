namespace Magnum.Cryptography
{
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
}