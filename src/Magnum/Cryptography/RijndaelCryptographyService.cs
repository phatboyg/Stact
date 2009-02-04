using System.Security.Cryptography;
using System.Text;

namespace Magnum.Common.Cryptography
{
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
            byte[] r = t.TransformFinalBlock(GetBytes(plainText), 0, GetBytes(plainText).Length);

            return new EncryptionResult(GetString(r), _cipher.IV);
        }

        public string Decrypt(EncryptionResult cipherText)
        {
            _cipher.IV = cipherText.Iv;

            ICryptoTransform t = _cipher.CreateDecryptor();
            byte[] cipherBytes = GetBytes(cipherText.CipherText);
            byte[] r = t.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return GetString(r);
        }

        string GetString(byte[] b)
        {
            return Encoding.Default.GetString(b);
        }


        byte[] GetBytes(string s)
        {
            return Encoding.Default.GetBytes(s);
        }

        public void Dispose()
        {
            _cipher.Clear();
        }
    }
}