namespace Magnum.CommandLine
{
    using System;
    using System.Collections.Generic;
    using Reflection;

    public class ArgumentParsingInstructions<ARGS> where ARGS : new()
    {
        private List<FastProperty<ARGS>> _positionalArguments;
        private Dictionary<string, FastProperty<ARGS>> _shortFormArguments;
        private Dictionary<string, FastProperty<ARGS>> _longFormArguments;
        private int _numberOfAguments = 0;
        private readonly ShortFormConvention _shortFormConvention = new ShortFormConvention();
        private readonly LongFormConvention _longFormConvention = new LongFormConvention();

        public ArgumentParsingInstructions()
        {
            _positionalArguments = new List<FastProperty<ARGS>>();
            _shortFormArguments = new Dictionary<string, FastProperty<ARGS>>(StringComparer.InvariantCultureIgnoreCase);
            _longFormArguments = new Dictionary<string, FastProperty<ARGS>>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var info in typeof(ARGS).GetProperties())
            {
                var fp = new FastProperty<ARGS>(info);
                _positionalArguments.Add(fp);
                _shortFormArguments.Add(_shortFormConvention.Convert(info), fp);
                _longFormArguments.Add(_longFormConvention.Convert(info), fp);

                _numberOfAguments++;
            }
        }

        public ARGS Build(string[] arguments)
        {
            var result = new ARGS();
            int i = 0;
            foreach (var s in arguments)
            {
                if (i >= _numberOfAguments) break;

                var a = new Argument(s);
                if (a.IsPostional)
                    _positionalArguments[i].Set(result, a.Value);
                else 
                    if(a.IsShortForm)
                        _shortFormArguments[a.Name].Set(result, a.Value);
                    else
                        _longFormArguments[a.Name].Set(result, a.Value);

                i++;
            }
            return result;
        }
    }
}