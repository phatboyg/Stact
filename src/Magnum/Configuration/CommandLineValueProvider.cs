// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Configuration
{
    using System;
    using System.Collections.Generic;
    using CommandLineParser;
    using System.Linq;

    public class CommandLineValueProvider :
        ConfigurationValueProvider
    {
        readonly string _args;

        public CommandLineValueProvider(string args)
        {
            _args = args;
        }

        public string Name
        {
            get { return "Command Line"; }
        }

        ConfigurationDto[] ConfigurationValueProvider.GetEntries()
        {
            var parser = new MonadicCommandLineParser();
            var output = parser.Parse(_args);
            var result = new List<ConfigurationDto>();

            foreach(IDefinitionElement item in output)
            {
                var dto = new ConfigurationDto {Key = item.Key, Value = item.Value};
                result.Add(dto);
            }

            return result.ToArray();
            
        }
    }
}