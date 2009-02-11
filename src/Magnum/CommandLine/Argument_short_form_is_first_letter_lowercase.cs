namespace Magnum.CommandLine
{
    using System.Collections.Generic;
    using System.Reflection;
    using Reflection;

    public class Argument_short_form_is_first_letter_lowercase : 
        IArgumentNameConvention
    {
        public string Convert(PropertyInfo prop)
        {
            return prop.Name.ToLower()[0].ToString();
        }

        public void Append(PropertyInfo prop, IDictionary<string, FastProperty> args, FastProperty fp)
        {
            var key = GenerateKey(prop.Name.ToLower(), args);
            args.Add(key, fp);
        }

        private string GenerateKey(string propName, IDictionary<string, FastProperty> args)
        {
            string key = "";
            foreach (var c in propName)
            {
                key += c;
                if (!args.ContainsKey(key)) break;
            }
            return key;
        }
    }
}