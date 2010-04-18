namespace Magnum.Cryptography
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Extensions;

	public class DpapiCryptographyService :
        ICryptographyService
    {

        public void Dispose()
        {
            //no-op
        }

        public EncryptedText Encrypt(string clearText)
        {
            var clearBytes = Encoding.UTF8.GetBytes(clearText);
            var iv = GenerateIv();
            var cipherBytes = ProtectedData.Protect(clearBytes, iv, DataProtectionScope.LocalMachine);
            var result = new EncryptedText(cipherBytes, iv);
            return result;
        }

        public string Decrypt(EncryptedText cipherText)
        {
            var cipherBytes = cipherText.GetBytes();
            var iv = cipherText.Iv;
            var clearBytes = ProtectedData.Unprotect(cipherBytes, iv, DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(clearBytes);
        }

        public EncryptedStream Encrypt(Stream clearStream)
        {
            var clearBytes = clearStream.ReadToEnd();
            var iv = GenerateIv();
            var cipherBytes = ProtectedData.Protect(clearBytes, iv, DataProtectionScope.LocalMachine);
            var result = new EncryptedStream(cipherBytes, iv);
            return result;
        }

        public Stream Decrypt(EncryptedStream cipherStream)
        {
            var cipherBytes = cipherStream.GetBytes();
            var iv = cipherStream.Iv;
            var clearBytes = ProtectedData.Unprotect(cipherBytes, iv, DataProtectionScope.LocalMachine);
            return new MemoryStream(clearBytes);
        }

        byte[] GenerateIv()
        {
            using (var c = new RijndaelManaged()
            {
                KeySize = 256, // defaults to 256, it's better to be explicit.
                BlockSize = 256, // defaults to 128 bits, so let's set this to 256 for better security
                Mode = CipherMode.CBC,
                Padding = PaddingMode.ISO10126, // adds random padding bytes which reduces the predictability of the plain text
            })
            {
                c.GenerateIV();
                return c.IV;
            }
        }
    }
}