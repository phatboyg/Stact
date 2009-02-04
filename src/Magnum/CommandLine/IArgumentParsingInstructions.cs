namespace Magnum.CommandLine
{
    public interface IArgumentParsingInstructions
    {
        object Build(string[] arguments);
    }
}