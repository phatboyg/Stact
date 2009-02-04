namespace Magnum.Specs.CommandLine
{
    using System.Reflection;

    public class ShortFormConvention
    {
        public string Convert(PropertyInfo prop)
        {
            return prop.Name.ToLower()[0].ToString();
        }
    }
}