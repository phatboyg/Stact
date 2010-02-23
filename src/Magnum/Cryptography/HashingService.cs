namespace Magnum.Cryptography
{
    public interface HashingService
    {
        string Hash(string clearText);
        byte[] Hash(byte[] clearBytes);
    }
}