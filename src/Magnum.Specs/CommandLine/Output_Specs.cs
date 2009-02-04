namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;
    using MbUnit.Framework;

    public class Output_Specs
    {
        [Test]
        [Repeat(300)] //5.04
        public void NAME()
        {
            OutputV2 o = new OutputV2();
            o.CommandName = "test";
            o.Command = new TestCommand<NullArgs>();
            o.ParsedArguments = new NullArgs();

            TestCommand<NullArgs>.WasExecuted.ShouldBeFalse();

            o.Execute();

            TestCommand<NullArgs>.WasExecuted.ShouldBeTrue();
            TestCommand<NullArgs>.WasExecuted = false;
        }

        [Test]
        [Repeat(300)] //4.98
        public void V1()
        {
            ParsedCommandLineOutput<NullArgs> a = new ParsedCommandLineOutput<NullArgs>();
            a.CommandName = "test";
            a.Command = new TestCommand<NullArgs>();
            a.Args = new NullArgs();

            TestCommand<NullArgs>.WasExecuted.ShouldBeFalse();

            a.Execute();

            TestCommand<NullArgs>.WasExecuted.ShouldBeTrue();
            TestCommand<NullArgs>.WasExecuted = false;
        }
    }
}