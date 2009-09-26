namespace Magnum.Configuration.Client
{
    public interface IReadWriteSettings :
        IReadOnlySettings
    {
        void Put(string key, string value);
    }
}