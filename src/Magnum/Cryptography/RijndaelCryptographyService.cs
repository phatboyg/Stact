using System.Security.Cryptography;
using System.Text;

namespace Magnum.Cryptography
{
    using System;
    using System.IO;

    //http://www.developerfusion.co.uk/show/4647/4/

    public class RijndaelCryptographyService :
        ICryptographyService
    {
        //a block cipher adopted as an encryption
        //standard by the U.S. government.
        //http://en.wikipedia.org/wiki/Rijndael
        private RijndaelManaged _cipher;

        public RijndaelCryptographyService(string key)
        {
            _cipher = new RijndaelManaged();

            // Set the key and block size.
            // Although the key size defaults to 256, it's better to be explicit.
            _cipher.KeySize = 256;

            // BlockSize defaults to 128 bits, so let's set this
            // to 256 for better security
            _cipher.BlockSize = 256;


            //CBC creates each cipher text block by first XORing the plain text
            //block with the previous cipher text block and then encrypting this
            //combined block. 
            _cipher.Mode = CipherMode.CBC;


            //using the ISO10126 mode as it adds random padding bytes, which reduces the predictability of the plain text
            _cipher.Padding = PaddingMode.ISO10126;


            _cipher.Key = GetBytes(key);
        }

        public EncryptionResult Encrypt(string plainText)
        {
            _cipher.GenerateIV();

            ICryptoTransform t = _cipher.CreateEncryptor();
            var plainBytes = GetBytes(plainText);
            var r = t.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            var cipherString = GetCipherString(r);

            return new EncryptionResult(cipherString, _cipher.IV);
        }

        public EncryptionStreamResult Encrypt(Stream plainStream)
        {
            _cipher.GenerateIV();

            ICryptoTransform t = _cipher.CreateEncryptor();
            var plainBytes = GetBytes(plainStream);
            var r = t.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            var cipherStream = GetStream(r);

            return new EncryptionStreamResult(cipherStream, _cipher.IV);
        }

        public string Decrypt(EncryptionResult cipherText)
        {
            _cipher.IV = cipherText.Iv;

            ICryptoTransform t = _cipher.CreateDecryptor();
            byte[] cipherBytes = GetCipherBytes(cipherText.CipherText);
            byte[] r = t.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return GetString(r);
        }

        public Stream Decrypt(EncryptionStreamResult cipherStream)
        {
            _cipher.IV = cipherStream.Iv;

            ICryptoTransform t = _cipher.CreateDecryptor();
            byte[] cipherBytes = GetBytes(cipherStream.CipherStream);
            byte[] r = t.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return GetStream(r);
        }

        string GetString(byte[] b)
        {
            return Encoding.UTF8.GetString(b);
        }
        string GetCipherString(byte[] b)
        {
            return Convert.ToBase64String(b);
        }

        Stream GetStream(byte[] b)
        {
            return new MemoryStream(b);
        }

        byte[] GetBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
        byte[] GetCipherBytes(string s)
        {
            return Convert.FromBase64String(s);
        }

        byte[] GetBytes(Stream s)
        {
            var bytes = new byte[s.Length];
            s.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public void Dispose()
        {
            _cipher.Clear();
        }
    }
}