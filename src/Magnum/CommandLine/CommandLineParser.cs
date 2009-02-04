namespace Magnum.CommandLine
{
    using System.Collections.Generic;

    public class CommandLineParser
    {
        private Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        private IArgumentOrderPolicy _policy = new Arguments_must_be_positional_then_named();


        public object Parse(string[] commandLine)
        {
            var commandName = commandLine.Head();
            string[] remainder = commandLine.Tail();

            object command = _commands[commandName];

            new Arguments_must_be_positional_then_named().Verify(remainder);

            return null;
        }
        public Output<ARGS> Parse<ARGS>(string[] commandLine) where ARGS : new()
        {
            Output<ARGS> result = new Output<ARGS>();
            result.CommandName = commandLine.Head();
            result.Command = (IArgCommand<ARGS>)_commands[result.CommandName];
            string[] remainder = commandLine.Tail();

            _policy.Verify(remainder);

            result.Args = new ArgumentParsingInstructions<ARGS>().Build(remainder);
            return result;
        }


        public void AddCommand<COMMAND, ARGS>() where COMMAND : IArgCommand<ARGS>, new() where ARGS : new()
        {
            string key = new Use_types_name_lowercased_removing_Command().GetName<COMMAND>();
            var api = new ArgumentParsingInstructions<ARGS>();
            _commands.Add(key, new COMMAND());
        }
    }
}