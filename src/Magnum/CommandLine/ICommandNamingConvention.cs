namespace Magnum.CommandLine
{
    using System;

    public interface ICommandNamingConvention
    {
        string GetName<T>();
        string GetName(Type t);
    }
}