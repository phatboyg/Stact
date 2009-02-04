namespace Magnum.CommandLine
{
    using System.Collections.Generic;

    public class CommandLineParser
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        private readonly IArgumentOrderPolicy _policy = new Arguments_must_be_positional_then_named();
        private readonly ICommandNamingConvention _commandNamingConvention = new Use_types_name_lowercased_removing_Command();


        public object Parse(string[] commandLine)
        {
            var commandName = commandLine.Head();
            string[] remainder = commandLine.Tail();

            object command = _commands[commandName];

            new Arguments_must_be_positional_then_named().Verify(remainder);

            return null;
        }
        public ParsedCommandLineOutput<ARGS> Parse<ARGS>(string[] commandLine) where ARGS : new()
        {
            ParsedCommandLineOutput<ARGS> result = new ParsedCommandLineOutput<ARGS>();
            result.CommandName = commandLine.Head();
            result.Command = (IArgCommand<ARGS>)_commands[result.CommandName];
            string[] remainder = commandLine.Tail();

            _policy.Verify(remainder);

            result.Args = new ArgumentParsingInstructions<ARGS>().Build(remainder);
            return result;
        }


        public void AddCommand<COMMAND>() where COMMAND : ICommand, new()
        {
            string key = _commandNamingConvention.GetName<COMMAND>();
            _commands.Add(key, new COMMAND());
        }
    }
}