namespace Magnum.CommandLine
{
    using System;
    using System.Reflection;

    public class ParsedCommandLineOutput<ARGS> where ARGS : new()
    {
        public string CommandName { get; set; }
        public IArgCommand<ARGS> Command { get; set; }
        public ARGS Args { get; set; }

        public void Execute()
        {
            Command.Execute(Args);
        }
    }

    public class OutputV2
    {
        public string CommandName { get; set; }
        public ICommand Command { get; set; }
        public object ParsedArguments { get; set; }

        public void Execute()
        {
            new FancyTypeCoercionSoIDontHaveToSeeItElsewhere(Command, ParsedArguments).Execute();
        }
    }

    public class FancyTypeCoercionSoIDontHaveToSeeItElsewhere
    {
        private readonly ICommand _command;
        private readonly object _arguments;
        private Type _commandType;
        private Type _argumentType;
        private Type _closedGenericForm;

        public FancyTypeCoercionSoIDontHaveToSeeItElsewhere(ICommand command, object arguments)
        {
            _command = command;
            _arguments = arguments;

            _commandType = command.GetType();
            _argumentType = arguments.GetType();
            _closedGenericForm = typeof(IArgCommand<>).MakeGenericType(_argumentType);
        }

        public void Execute()
        {
            IsCommandOkWith(_closedGenericForm, _commandType);

            MethodInfo mi = _closedGenericForm.GetMethod("Execute");
            mi.Invoke(_command, new[] { _arguments });
        }
        

        private void IsCommandOkWith(Type closedGenericForm, Type commandType)
        {
            if(!closedGenericForm.IsAssignableFrom(commandType))
                throw new Exception(string.Format("Can't convert '{0}' to '{1}'", commandType, closedGenericForm));
        }
    }
}