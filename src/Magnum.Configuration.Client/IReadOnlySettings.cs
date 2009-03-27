namespace Magnum.Configuration.Client
{
    public interface IReadOnlySettings
    {
        ISetting Get(string key);
    }
}