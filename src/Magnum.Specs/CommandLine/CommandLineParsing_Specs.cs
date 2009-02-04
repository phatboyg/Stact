namespace Magnum.Specs.CommandLine
{
    using MbUnit.Framework;

    public class CommandLineParsing_Specs
    {
        private string[] _soloCommand = new []{"test"};
        private string[] _commandAndOnePositionalArgument = new[] {"test","magnum"};
        private string[] _commandAndTwoPositionalArguments = new[] {"test", "magnum", "local"};
        private string[] _commandAndOneNamedArgument = new[] {"test","--Location:local"};

        [Test]
        public void Should_Parse_Just_Command()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<NullArgs>, NullArgs>();

            Output<NullArgs> o = p.Parse<NullArgs>(_soloCommand);
            o.CommandName.ShouldEqual("test");
            o.Command.ShouldBeType<TestCommand<NullArgs>>();

            o = p.Parse<NullArgs>(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("test");

            o = p.Parse<NullArgs>(_commandAndTwoPositionalArguments);
            o.CommandName.ShouldEqual("test");
        }

        [Test]
        public void Should_Parse_Command_and_Postional_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<OneArgument>, OneArgument>();
            Output<OneArgument> o = p.Parse<OneArgument>(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("test");
            o.Args.Name.ShouldEqual("magnum");
        }

        [Test]
        public void Should_parse_command_and_2_postional_arguments()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<TwoArguments>, TwoArguments>();
            Output<TwoArguments> two = p.Parse<TwoArguments>(_commandAndTwoPositionalArguments);
            two.Args.Name.ShouldEqual("magnum");
            two.Args.Location.ShouldEqual("local");
        }

        [Test]
        public void Should_Parse_Command_And_One_Named_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<TwoArguments>, TwoArguments>();

            Output<TwoArguments> two = p.Parse<TwoArguments>(_commandAndOneNamedArgument);
            two.Args.Name.ShouldBeNull();
            two.Args.Location.ShouldEqual("local");
        }
    }
}