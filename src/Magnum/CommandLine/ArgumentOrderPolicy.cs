namespace Magnum.CommandLine
{
    using System;
    using System.Collections.Generic;

    public class ArgumentOrderPolicy
    {
        public void Verify(string[] arguments)
        {
            List<Argument> argus = new List<Argument>();

            foreach (var s in arguments)
                argus.Add(new Argument(s));

            bool foundNamedArgument = false;
            foreach (var argument in argus)
            {
                if (!argument.IsPostional)
                    foundNamedArgument = true;
                
                if(foundNamedArgument && argument.IsPostional)
                    throw new Exception("You can't have a positional argument after a named argument"); 
            }
        }
    }
}