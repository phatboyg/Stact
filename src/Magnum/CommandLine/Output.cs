namespace Magnum.CommandLine
{
    public class Output<ARGS> where ARGS : new()
    {
        public string CommandName { get; set; }
        public IArgCommand<ARGS> Command { get; set; }
        public ARGS Args { get; set; }

        public void Execute()
        {
            Command.Excute(Args);
        }
    }
}