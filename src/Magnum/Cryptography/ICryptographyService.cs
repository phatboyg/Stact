using System;

namespace Magnum.Cryptography
{
    using System.IO;

    public interface ICryptographyService :
        IDisposable
    {
        EncryptionResult Encrypt(string plainText);
        string Decrypt(EncryptionResult cipherText);

        EncryptionStreamResult Encrypt(Stream plainStream);
        Stream Decrypt(EncryptionStreamResult cipherStream);
    }
}