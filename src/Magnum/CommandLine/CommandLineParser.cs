namespace Magnum.CommandLine
{
    using System.Collections.Generic;

    public class CommandLineParser
    {
        private readonly Dictionary<string, Thing> _commands = new Dictionary<string, Thing>();
        private readonly IArgumentOrderPolicy _policy = new Arguments_must_be_positional_then_named();
        private readonly ICommandNamingConvention _commandNamingConvention = new Use_types_name_lowercased_removing_Command();


        public ParsedCommandLineOutput Parse(string[] commandLine)
        {
            var commandName = commandLine.Head();
            string[] remainder = commandLine.Tail();

            ICommand command = _commands[commandName].Command;

            _policy.Verify(remainder);
            object args = _commands[commandName].ParsingInstructions.Build(remainder);

            return new ParsedCommandLineOutput(){Command = command, CommandName = commandName, ParsedArguments = args};
        }


        public void AddCommand<CommandType, ArgsType>() where CommandType : IArgCommand<ArgsType>, new() where ArgsType : new()
        {
            string key = _commandNamingConvention.GetName<CommandType>();
            var argType = typeof (ArgsType);
            IArgumentParsingInstructions api = new ArgumentParsingInstructions(argType);
            var thing = new Thing {Command = new CommandType(), ParsingInstructions = api};
            _commands.Add(key, thing);
        }
    }

    public class Thing
    {
        public IArgumentParsingInstructions ParsingInstructions { get; set; }
        public ICommand Command { get; set; }
    }
}