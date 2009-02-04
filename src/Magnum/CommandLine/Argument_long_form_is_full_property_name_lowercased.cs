namespace Magnum.CommandLine
{
    using System.Reflection;

    public class Argument_long_form_is_full_property_name_lowercased :
        IArgumentNameConvention
    {
        public string Convert(PropertyInfo prop)
        {
            return prop.Name.ToLower();
        }
    }
}