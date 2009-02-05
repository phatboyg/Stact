namespace Magnum.CommandLine
{
    using System;
    using System.Text.RegularExpressions;

    public class Use_types_name_lowercased_removing_Args_or_Arguments :
        IArgumentCommandNameConvention
    {
        public string GetName<T>()
        {
            return GetName(typeof(T));
        }

        public string GetName(Type t)
        {
            string result = t.Name.Replace("Arguments", "");
            result = result.Replace("Args", "");
            

            if (t.IsGenericType)
            {
                result = new Regex("`\\w*").Replace(result, "");
            }
            return result.ToLower();
        }
    }
}