namespace Magnum.CommandLine
{
    using System.Reflection;

    public class Argument_short_form_is_first_letter_lowercase : 
        IArgumentNameConvention
    {
        public string Convert(PropertyInfo prop)
        {
            return prop.Name.ToLower()[0].ToString();
        }
    }
}