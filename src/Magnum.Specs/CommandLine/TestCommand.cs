namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;

    public class TestCommand<Args> :
        IArgCommand<Args> where Args : new()
    {
        public static bool WasExecuted = false;

        public void Execute(Args args)
        {
            WasExecuted = true;
        }
    }

    public class Test2Command<Args> :
        IArgCommand<Args> where Args : new()
    {
        public static bool WasExecuted = false;

        public void Execute(Args args)
        {
            WasExecuted = true;
        }
    }
}