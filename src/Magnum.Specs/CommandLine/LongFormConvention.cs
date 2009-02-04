namespace Magnum.Specs.CommandLine
{
    using System.Reflection;

    public class LongFormConvention
    {
        public string Convert(PropertyInfo prop)
        {
            return prop.Name.ToLower();
        }
    }
}