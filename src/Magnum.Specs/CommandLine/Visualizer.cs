namespace Magnum.Specs.CommandLine
{
    using System;
    using Magnum.CommandLine;
    using NUnit.Framework;

    public class Visualizer
    {
        [Test]
        public void Show()
        {
            CommandLineParser p = new CommandLineParser();
            p.RegisterArgumentsForCommand<ArgumentWithSameFirstLetter>("test");
            
            Console.Write(p.WhatsRegistered());
        }
    }
}