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
    using System.Diagnostics;
    using Magnum.CommandLineParser;
    using NUnit.Framework;

    [TestFixture]
    public class Parsing_a_command_line
    {
        [Test]
        public void Should_return_an_enumeration_of_elements()
        {
            string commandLine = "update -a -b -d:true -e \"frank\" -f \"go hornets\" [filename]";

            ICommandLineParser parser = new MonadicCommandLineParser();

            Trace.WriteLine("Command Line: " + commandLine);

            parser.Parse(commandLine)
                .Each(x => { Trace.WriteLine(x.ToString()); });
        }

    	[Test]
    	public void Should_handle_escape_characters_in_the_line()
    	{
			string commandLine = "cmd -file \"\\\"c:\\\\system\\\\something\\'s cooking.txt\\\"\"";

			ICommandLineParser parser = new MonadicCommandLineParser();

			Trace.WriteLine("Command Line: " + commandLine);

			parser.Parse(commandLine)
				.Each(x => { Trace.WriteLine(x.ToString()); });
    		
    	}

        [Test]
        public void dru()
        {
            string commandLine = "cmd -file";

            ICommandLineParser parser = new MonadicCommandLineParser();

            Trace.WriteLine("Command Line: " + commandLine);

            parser.Parse(commandLine)
                .Each(x => { Trace.WriteLine(x.ToString()); });

        }
    }
}