namespace Magnum.Cryptography.PKI
{
    using System;

    public interface IPkiCryptographyService :
        IDisposable
    {
        EncryptOutput Encrypt(EncryptInput input);
        DecryptOutput Decrypt(DecryptInput input);

        //generate key pair?
    }
}