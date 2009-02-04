namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;
    using MbUnit.Framework;

    public class CommandLineParsing_Specs
    {
        private string[] _soloCommand = new []{"test"};
        private string[] _commandAndOnePositionalArgument = new[] {"test","magnum"};
        private string[] _commandAndTwoPositionalArguments = new[] {"test", "magnum", "local"};
        private string[] _commandAndOneLongNamedArgument = new[] {"test","--Location:local"};
        private string[] _commandAndOneShortNamedArgument = new[] {"test","-l:local"};
        private string[] _commandAndOnePostionalAndOneShortNamedArgument = new[] {"test","magnum","-l:local"};

        [Test]
        public void Should_Parse_Just_Command()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<NullArgs>, NullArgs>();

            ParsedCommandLineOutput o = p.Parse(_soloCommand);
            o.CommandName.ShouldEqual("test");
            o.Command.ShouldBeType<TestCommand<NullArgs>>();

            o = p.Parse(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("test");

            o = p.Parse(_commandAndTwoPositionalArguments);
            o.CommandName.ShouldEqual("test");
        }

        [Test]
        public void Should_Parse_Command_and_Postional_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<OneArgument>, OneArgument>();
            ParsedCommandLineOutput o = p.Parse(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("test");
            ((OneArgument)o.ParsedArguments).Name.ShouldEqual("magnum");
        }

        [Test]
        public void Should_parse_command_and_2_postional_arguments()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<TwoArguments>,TwoArguments>();

            ParsedCommandLineOutput two = p.Parse(_commandAndTwoPositionalArguments);
            ((TwoArguments)two.ParsedArguments).Name.ShouldEqual("magnum");
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }

        [Test]
        public void Should_Parse_Command_And_One_Long_Named_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<TwoArguments>, TwoArguments>();

            ParsedCommandLineOutput two = p.Parse(_commandAndOneLongNamedArgument);
            ((TwoArguments)two.ParsedArguments).Name.ShouldBeNull();
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }

        [Test]
        public void Should_Parse_Command_And_One_Short_Named_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<TwoArguments>, TwoArguments>();

            ParsedCommandLineOutput two = p.Parse(_commandAndOneShortNamedArgument);
            ((TwoArguments)two.ParsedArguments).Name.ShouldBeNull();
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }

        [Test]
        public void Should_Parse_positional_and_Named_args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<TwoArguments>, TwoArguments>();

            ParsedCommandLineOutput two = p.Parse(_commandAndOnePostionalAndOneShortNamedArgument);
            ((TwoArguments)two.ParsedArguments).Name.ShouldEqual("magnum");
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }
    }
}