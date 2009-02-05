namespace Magnum.CommandLine
{
    public interface IArgumentOrderPolicy
    {
        void Verify(string[] arguments);
    }
}