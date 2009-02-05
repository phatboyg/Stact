namespace Magnum.CommandLine
{
    using System;

    public interface IArgumentCommandNameConvention
    {
        string GetName<T>();
        string GetName(Type type);

    }
}