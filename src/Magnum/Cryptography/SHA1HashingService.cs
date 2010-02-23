namespace Magnum.Cryptography
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public class SHA1HashingService :
        HashingService
    {
        public string Hash(string clearText)
        {
            using(var s = new SHA1Managed())
            {
                var clearBytes = Encoding.UTF8.GetBytes(clearText);
                var cipherBytes = s.ComputeHash(clearBytes);
                return Convert.ToBase64String(cipherBytes);
            }
        }

        public byte[] Hash(byte[] clearBytes)
        {
            using (var s = new SHA1Managed())
            {
                var cipherBytes = s.ComputeHash(clearBytes);
                return cipherBytes;
            }
        }
    }
}