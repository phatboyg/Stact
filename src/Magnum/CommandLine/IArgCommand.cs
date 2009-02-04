namespace Magnum.CommandLine
{
    public interface IArgCommand<Args> : 
        ICommand where Args : new()

    {
        void Execute(Args args);
    }
}