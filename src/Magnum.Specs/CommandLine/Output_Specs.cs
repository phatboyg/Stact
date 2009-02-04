namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;
    using MbUnit.Framework;

    public class Output_Specs
    {
        [Test]
        [Repeat(300)]
        public void NAME()
        {
            ParsedCommandLineOutput o = new ParsedCommandLineOutput();
            o.CommandName = "test";
            o.Command = new TestCommand<NullArgs>();
            o.ParsedArguments = new NullArgs();

            TestCommand<NullArgs>.WasExecuted.ShouldBeFalse();

            o.Execute();

            TestCommand<NullArgs>.WasExecuted.ShouldBeTrue();
            TestCommand<NullArgs>.WasExecuted = false;
        }
    }
}