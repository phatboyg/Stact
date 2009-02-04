namespace Magnum.CommandLine
{
    using System.Text.RegularExpressions;

    public class Argument
    {
        public bool IsPostional { get; set; }
        public bool IsShortForm { get; set; }

        public string Name { get; set; }
        public string Value { get; set; }
        private readonly Regex _extract;


        public Argument(string arg)
        {
            IsPostional = !arg.StartsWith("-");

            _extract = new Regex("(?<form>-+)(?<name>\\w*):\"?(?<value>[a-zA-Z0-9 ]*)\"?");

            if (IsPostional)
                Value = arg;
            else
            {
                var m = _extract.Match(arg);
                if(m.Success)
                {
                    IsShortForm = m.Groups["form"].Value == "-" ? true : false;
                    Name = m.Groups["name"].Value;
                    Value = m.Groups["value"].Value;
                }
            }
        }
    }
}