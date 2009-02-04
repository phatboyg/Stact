namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;
    using MbUnit.Framework;

    public class AddingCommands_Specs
    {
        [Test]
        public void NAME()
        {
            CommandLineParser clp = new CommandLineParser();
            clp.AddCommand<TestCommand<NullArgs>,NullArgs>();
            clp.AddCommand<Test2Command<NullArgs>,NullArgs>();

            TestCommand<NullArgs>.WasExecuted.ShouldBeFalse();
            Test2Command<NullArgs>.WasExecuted.ShouldBeFalse();

            var o = clp.Parse<NullArgs>(new[] {"test"});
            o.Execute();

            TestCommand<NullArgs>.WasExecuted.ShouldBeTrue();
            Test2Command<NullArgs>.WasExecuted.ShouldBeFalse();
        }

        [Test]
        public void NAME2()
        {
            CommandLineParser clp = new CommandLineParser();
            clp.AddCommand<TestCommand<NullArgs>, NullArgs>();
            clp.AddCommand<Test2Command<NullArgs>, NullArgs>();

            TestCommand<NullArgs>.WasExecuted.ShouldBeFalse();
            Test2Command<NullArgs>.WasExecuted.ShouldBeFalse();

            var o = clp.Parse<NullArgs>(new[] { "test2" });
            o.Execute();

            TestCommand<NullArgs>.WasExecuted.ShouldBeFalse();
            Test2Command<NullArgs>.WasExecuted.ShouldBeTrue();
        }
    }
}