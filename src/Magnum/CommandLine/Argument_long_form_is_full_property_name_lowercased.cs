namespace Magnum.CommandLine
{
    using System.Collections.Generic;
    using System.Reflection;
    using Reflection;

    public class Argument_long_form_is_full_property_name_lowercased :
        IArgumentNameConvention
    {
        public string Convert(PropertyInfo prop)
        {
            return prop.Name.ToLower();
        }

        public void Append(PropertyInfo prop, IDictionary<string, FastProperty> args, FastProperty fp)
        {
            args.Add(prop.Name.ToLower(), fp);
        }
    }
}