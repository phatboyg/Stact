namespace Magnum.CommandLine
{
    using System.Collections.Generic;
    using System.Reflection;
    using Reflection;

    public interface IArgumentNameConvention
    {
        string Convert(PropertyInfo prop);
        void Append(PropertyInfo prop, IDictionary<string, FastProperty> args, FastProperty fp);
    }
}