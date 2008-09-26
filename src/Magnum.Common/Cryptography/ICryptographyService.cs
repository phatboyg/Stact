using System;

namespace Magnum.Common.Cryptography
{
    public interface ICryptographyService :
        IDisposable
    {
        EncryptionResult Encrypt(string plainText);
        string Decrypt(EncryptionResult cipherText);
    }
}