namespace Magnum.Common.Specs.CommandLine
{
    using MbUnit.Framework;

    public class CommandLineParsing_Specs
    {
        private string[] _soloCommand = new []{"update"};
        private string[] _commandAndOnePositionalArgument = new[] {"install","magnum"};
        private string[] _commandAndTwoPositionalArguments = new[] {"install", "magnum", "local"};

        [Test]
        public void Should_Parse_Just_Command()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<NullArgs>, NullArgs>();

            Output<NullArgs> o = p.Parse<NullArgs>(_soloCommand);
            o.CommandName.ShouldEqual("update");

            o = p.Parse<NullArgs>(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("install");

            o = p.Parse<NullArgs>(_commandAndTwoPositionalArguments);
            o.CommandName.ShouldEqual("install");
        }

        [Test]
        public void Should_Parse_Command_and_Postional_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<OneArgument>, OneArgument>();
            Output<OneArgument> o = p.Parse<OneArgument>(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("install");
            o.Args.Name.ShouldEqual("magnum");

            Output<TwoArguments> two = p.Parse<TwoArguments>(_commandAndTwoPositionalArguments);
            two.Args.Name.ShouldEqual("magnum");
            two.Args.Location.ShouldEqual("local");
        }
    }
}