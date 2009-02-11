namespace Magnum.CommandLine
{
    using System;
    using System.Collections.Generic;
    using Reflection;

    public class ArgumentParsingInstructions :
        IArgumentParsingInstructions
    {
        private List<FastProperty> _positionalArguments;
        private Dictionary<string, FastProperty> _shortFormArguments;
        private Dictionary<string, FastProperty> _longFormArguments;

        private int _numberOfAguments = 0;
        private readonly IArgumentNameConvention _shortFormConvention = new Argument_short_form_is_first_letter_lowercase();
        private readonly IArgumentNameConvention _longFormConvention = new Argument_long_form_is_full_property_name_lowercased();
        private Type _argumentType;

        public ArgumentParsingInstructions(Type argumentType)
        {
            _argumentType = argumentType;
            _positionalArguments = new List<FastProperty>();
            _shortFormArguments = new Dictionary<string, FastProperty>(StringComparer.InvariantCultureIgnoreCase);
            _longFormArguments = new Dictionary<string, FastProperty>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var info in _argumentType.GetProperties())
            {
                var fp = new FastProperty(info);
                _positionalArguments.Add(fp);

                _shortFormConvention.Append(info, _shortFormArguments, fp);
                _longFormConvention.Append(info, _longFormArguments, fp);

                _numberOfAguments++;
            }
        }

        public object Build(string[] arguments)
        {
            var result = Activator.CreateInstance(_argumentType);

            int i = 0;
            foreach (var s in arguments)
            {
                if (i >= _numberOfAguments) break;

                var a = new Argument(s);
                if (a.IsPostional)
                    _positionalArguments[i].Set(result, a.Value);
                else
                    if (a.IsShortForm)
                        _shortFormArguments[a.Name].Set(result, a.Value);
                    else
                        _longFormArguments[a.Name].Set(result, a.Value);

                i++;
            }
            return result;
        }

        public string WhatAreTheOptions()
        {
            //foreach argument display
            // long form, short form, type
            // how to link long and short form?

            return "";
        }
    }
}