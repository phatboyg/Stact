namespace Magnum.Specs.CommandLine
{
    using Magnum.CommandLine;

    public class TestCommand<Args> :
        IArgCommand<Args> where Args : new()
    {
        public void Excute(Args args)
        {
            
        }
    }
}