namespace Magnum.Common.Specs.CommandLine
{
    public interface IArgCommand<Args> : 
        ICommand where Args : new()

    {
        void Excute(Args args);
    }
}