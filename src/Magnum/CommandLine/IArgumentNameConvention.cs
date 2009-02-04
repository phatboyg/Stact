namespace Magnum.CommandLine
{
    using System.Reflection;

    public interface IArgumentNameConvention
    {
        string Convert(PropertyInfo prop);
    }
}