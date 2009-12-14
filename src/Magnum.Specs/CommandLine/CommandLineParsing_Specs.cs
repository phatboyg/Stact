namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;
    using NUnit.Framework;
    using TestFramework;

	public class CommandLineParsing_Specs
    {
        private string[] _soloCommand = new []{"test"};
        private string[] _commandAndOnePositionalArgument = new[] {"test","magnum"};
        private string[] _commandAndTwoPositionalArguments = new[] {"test", "magnum", "local"};
        private string[] _commandTwoAndTwoPositionalArguments = new[] {"two", "magnum", "local"};
        private string[] _commandAndOneLongNamedArgument = new[] {"test","--Location:local"};
        private string[] _commandAndOneShortNamedArgument = new[] {"test","-l:local"};
        private string[] _commandAndOnePostionalAndOneShortNamedArgument = new[] {"test","magnum","-l:local"};

        [Test]
        public void Should_Parse_Just_Command()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<NullArgs>("test");

            ParsedCommandLineOutput o = p.Parse(_soloCommand);
            o.CommandName.ShouldEqual("test");
			o.ParsedArguments.ShouldBeAnInstanceOf<NullArgs>();

            o = p.Parse(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("test");

            o = p.Parse(_commandAndTwoPositionalArguments);
            o.CommandName.ShouldEqual("test");
        }

        [Test]
        public void AutoNaming()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArguments<TwoArguments>();

            var o = p.Parse(_commandTwoAndTwoPositionalArguments);
            o.CommandName.ShouldEqual("two");
			o.ParsedArguments.ShouldBeAnInstanceOf<TwoArguments>();
        }

        [Test]
        public void Should_Parse_Command_and_Postional_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<OneArgument>("test");
            ParsedCommandLineOutput o = p.Parse(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("test");
            ((OneArgument)o.ParsedArguments).Name.ShouldEqual("magnum");
        }

        [Test]
        public void Should_parse_command_and_2_postional_arguments()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<TwoArguments>("test");

            ParsedCommandLineOutput two = p.Parse(_commandAndTwoPositionalArguments);
            ((TwoArguments)two.ParsedArguments).Name.ShouldEqual("magnum");
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }

        [Test]
        public void Should_Parse_Command_And_One_Long_Named_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<TwoArguments>("test");

            ParsedCommandLineOutput two = p.Parse(_commandAndOneLongNamedArgument);
            ((TwoArguments)two.ParsedArguments).Name.ShouldBeNull();
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }

        [Test]
        public void Should_Parse_Command_And_One_Short_Named_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<TwoArguments>("test");

            ParsedCommandLineOutput two = p.Parse(_commandAndOneShortNamedArgument);
            ((TwoArguments)two.ParsedArguments).Name.ShouldBeNull();
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }

        [Test]
        public void Should_Parse_positional_and_Named_args()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<TwoArguments>("test");

            ParsedCommandLineOutput two = p.Parse(_commandAndOnePostionalAndOneShortNamedArgument);
            ((TwoArguments)two.ParsedArguments).Name.ShouldEqual("magnum");
            ((TwoArguments)two.ParsedArguments).Location.ShouldEqual("local");
        }

        [Test]
        public void Should_be_able_handle_args_with_same_first_letter()
        {
            var args = new[] {"test", "-n:bob","-nu:bill"};
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<ArgumentWithSameFirstLetter>("test");

            ParsedCommandLineOutput o = p.Parse(args);
            ((ArgumentWithSameFirstLetter)o.ParsedArguments).Name.ShouldEqual("bob");
            ((ArgumentWithSameFirstLetter)o.ParsedArguments).Number.ShouldEqual("bill");
        }

        //partial longform?
    }
}