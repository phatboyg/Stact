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
namespace Magnum.Specs.CommandLineParser
{
	using System;
	using System.Diagnostics;
    using System.Linq;
    using Magnum.CommandLineParser;
    using NUnit.Framework;
    using TestFramework;

	[TestFixture]
    public class Parsing_a_command_line
    {
        [Test]
        public void Should_return_an_enumeration_of_elements()
        {
            string commandLine = "update -a -b -d:true -e \"frank\" -f \"go hornets\" [filename]";

            ICommandLineParser parser = new MonadicCommandLineParser();

            Trace.WriteLine("Command Line: " + commandLine);

        	var elements = parser.Parse(commandLine).ToArray();

			Trace.WriteLine(string.Join(Environment.NewLine, elements.Select(x => x.ToString()).ToArray()));
        	
			elements.Count().ShouldEqual(7);
			elements[0].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[1].ShouldBeAnInstanceOf<SwitchElement>();
			elements[2].ShouldBeAnInstanceOf<SwitchElement>();
			elements[3].ShouldBeAnInstanceOf<DefinitionElement>();
			elements[4].ShouldBeAnInstanceOf<DefinitionElement>();
			elements[5].ShouldBeAnInstanceOf<DefinitionElement>();
			elements[6].ShouldBeAnInstanceOf<TokenElement>();
        }

    	[Test]
    	public void Should_handle_escape_characters_in_the_line()
    	{
			string commandLine = "cmd -file \"\\\"c:\\\\system\\\\something\\'s cooking.txt\\\"\"";

			ICommandLineParser parser = new MonadicCommandLineParser();

			Trace.WriteLine("Command Line: " + commandLine);

			var elements = parser.Parse(commandLine).ToArray();

			Trace.WriteLine(string.Join(Environment.NewLine, elements.Select(x => x.ToString()).ToArray()));

			elements.Count().ShouldEqual(2);
			elements[0].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[1].ShouldBeAnInstanceOf<DefinitionElement>();

    		((DefinitionElement) elements[1]).Value.ShouldEqual("\"" + @"c:\system\something's cooking.txt" + "\"");
    	}

        [Test]
        public void dru()
        {
            string commandLine = "cmd -file";

            ICommandLineParser parser = new MonadicCommandLineParser();

            Trace.WriteLine("Command Line: " + commandLine);

			var elements = parser.Parse(commandLine).ToArray();

			Trace.WriteLine(string.Join(Environment.NewLine, elements.Select(x => x.ToString()).ToArray()));

			elements.Count().ShouldEqual(2);
			elements[0].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[1].ShouldBeAnInstanceOf<DefinitionElement>();
			((DefinitionElement)elements[1]).Value.ShouldEqual("");
		}

    	[Test]
    	public void Should_handle_a_nested_set_of_commands_and_flags()
    	{
    		string commandLine = "config --global net.framework net-3.5";

			Trace.WriteLine("Command Line: " + commandLine);
			
			ICommandLineParser parser = new MonadicCommandLineParser();

			var elements = parser.Parse(commandLine).ToArray();

			Trace.WriteLine(string.Join(Environment.NewLine, elements.Select(x => x.ToString()).ToArray()));

			elements.Count().ShouldEqual(4);
			elements[0].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[1].ShouldBeAnInstanceOf<SwitchElement>();
			elements[2].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[3].ShouldBeAnInstanceOf<ArgumentElement>();
		}

    	[Test]
    	public void A_git_style_command_should_be_supported()
    	{
    		string commandLine = "remote add dru git://github.com/drusellers/nu.git";

			Trace.WriteLine("Command Line: " + commandLine);
			
			ICommandLineParser parser = new MonadicCommandLineParser();

			var elements = parser.Parse(commandLine).ToArray();

			Trace.WriteLine(string.Join(Environment.NewLine, elements.Select(x => x.ToString()).ToArray()));

			elements.Count().ShouldEqual(4);
			elements[0].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[1].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[2].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[3].ShouldBeAnInstanceOf<ArgumentElement>();
		}

    	[Test]
    	public void A_simple_path_specification_should_be_allowed()
    	{
    		string commandLine = "add --all .";

			Trace.WriteLine("Command Line: " + commandLine);
			
			ICommandLineParser parser = new MonadicCommandLineParser();

			var elements = parser.Parse(commandLine).ToArray();

			Trace.WriteLine(string.Join(Environment.NewLine, elements.Select(x => x.ToString()).ToArray()));

			elements.Count().ShouldEqual(3);
			elements[0].ShouldBeAnInstanceOf<ArgumentElement>();
			elements[1].ShouldBeAnInstanceOf<SwitchElement>();
			elements[2].ShouldBeAnInstanceOf<ArgumentElement>();
		}
    }
}