using System;

namespace Magnum.Cryptography
{
    public interface ICryptographyService :
        IDisposable
    {
        EncryptionResult Encrypt(string plainText);
        string Decrypt(EncryptionResult cipherText);
    }
}