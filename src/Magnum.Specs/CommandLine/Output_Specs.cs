namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;
    using MbUnit.Framework;

    public class Output_Specs
    {
        [Test]
        public void NAME()
        {
            OutputV2 o = new OutputV2();
            o.CommandName = "test";
            o.Command = new TestCommand<NullArgs>();
            o.ParsedArguments = new NullArgs();

            TestCommand<NullArgs>.WasExecuted.ShouldBeFalse();

            o.Execute();

            TestCommand<NullArgs>.WasExecuted.ShouldBeTrue();
        }
    }
}