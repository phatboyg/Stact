namespace Magnum.Common.Specs.CommandLine
{
    using System.Collections.Generic;
    using Common.Reflection;

    public class CommandLineParser
    {
        private Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();


        public Output<ARGS> Parse<ARGS>(string[] commandLine) where ARGS : new()
        {
            Output<ARGS> result = new Output<ARGS>();
            result.Args = new ARGS();
            result.CommandName = commandLine.Head();
            string[] remainder = commandLine.Tail();

            //populate args here
            Populate(result.Args, remainder);
            return result;
        }

        private static void Populate<ARGS>(ARGS args, string[] remainder)
        {
            var props = typeof (ARGS).GetProperties();
            int i = 0;
            foreach (var prop in props)
            {
                FastProperty<ARGS, string> fp = new FastProperty<ARGS, string>(prop);
                fp.Set(args, remainder[i]);
                i++;
            }
        }

        public void AddCommand<COMMAND, ARGS>() where COMMAND : IArgCommand<ARGS>, new() where ARGS : new()
        {
            _commands.Add(typeof (COMMAND).Name.Replace("Command", ""), new COMMAND());
        }
    }
} 