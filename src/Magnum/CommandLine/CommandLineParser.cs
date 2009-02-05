namespace Magnum.CommandLine
{
    using System.Collections.Generic;

    public class CommandLineParser
    {
        private readonly Dictionary<string, Thing> _commands = new Dictionary<string, Thing>();
        private readonly IArgumentOrderPolicy _policy = new Arguments_must_be_positional_then_named();
        

        public ParsedCommandLineOutput Parse(string[] commandLine)
        {
            var commandName = commandLine.Head();
            string[] remainder = commandLine.Tail();

            _policy.Verify(remainder);
            object args = _commands[commandName].ParsingInstructions.Build(remainder);

            return new ParsedCommandLineOutput(){CommandName = commandName, ParsedArguments = args};
        }


        public void RegisterArgumentsForCommand<ArgsType>(string commandName) where ArgsType : new()
        {
            string key = commandName;
            var argType = typeof (ArgsType);
            IArgumentParsingInstructions api = new ArgumentParsingInstructions(argType);
            var thing = new Thing {ParsingInstructions = api};
            _commands.Add(key, thing);
        }
    }

    public class Thing
    {
        public IArgumentParsingInstructions ParsingInstructions { get; set; }
    }
}