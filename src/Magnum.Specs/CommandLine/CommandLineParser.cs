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
    using System.Collections.Generic;
    using Common.Reflection;

    public class CommandLineParser
    {
        private Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();


        public Output<ARGS> Parse<ARGS>(string[] commandLine) where ARGS : new()
        {
            Output<ARGS> result = new Output<ARGS>();
            result.Args = new ARGS();
            result.CommandName = commandLine.Head();
            string[] remainder = commandLine.Tail();

            //populate args here
            Populate(result.Args, remainder);
            return result;
        }

        private static void Populate<ARGS>(ARGS args, string[] remainder)
        {
            var props = typeof (ARGS).GetProperties();
            int i = 0;
            foreach (var prop in props)
            {
                FastProperty<ARGS, string> fp = new FastProperty<ARGS, string>(prop);
                fp.Set(args, remainder[i]);
                i++;
            }
        }

        public void AddCommand<COMMAND, ARGS>() where COMMAND : IArgCommand<ARGS>, new() where ARGS : new()
        {
            _commands.Add(typeof (COMMAND).Name.Replace("Command", ""), new COMMAND());
        }
    }
}