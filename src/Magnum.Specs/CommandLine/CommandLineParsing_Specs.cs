// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Specs.CommandLine
{
    using MbUnit.Framework;

    public class CommandLineParsing_Specs
    {
        private string[] _soloCommand = new []{"update"};
        private string[] _commandAndOnePositionalArgument = new[] {"install","magnum"};
        private string[] _commandAndTwoPositionalArguments = new[] {"install", "magnum", "local"};

        [Test]
        public void Should_Parse_Just_Command()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<NullArgs>, NullArgs>();

            Output<NullArgs> o = p.Parse<NullArgs>(_soloCommand);
            o.CommandName.ShouldEqual("update");

            o = p.Parse<NullArgs>(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("install");

            o = p.Parse<NullArgs>(_commandAndTwoPositionalArguments);
            o.CommandName.ShouldEqual("install");
        }

        [Test]
        public void Should_Parse_Command_and_Postional_Args()
        {
            CommandLineParser p = new CommandLineParser();
            p.AddCommand<TestCommand<OneArgument>, OneArgument>();
            Output<OneArgument> o = p.Parse<OneArgument>(_commandAndOnePositionalArgument);
            o.CommandName.ShouldEqual("install");
            o.Args.Name.ShouldEqual("magnum");

            Output<TwoArguments> two = p.Parse<TwoArguments>(_commandAndTwoPositionalArguments);
            two.Args.Name.ShouldEqual("magnum");
            two.Args.Location.ShouldEqual("local");
        }
    }
}