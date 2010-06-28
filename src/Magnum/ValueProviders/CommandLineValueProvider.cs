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
namespace Magnum.ValueProviders
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Collections;
	using CommandLineParser;


	public class CommandLineValueProvider :
		ValueProvider
	{
		readonly ValueProvider _provider;

		public CommandLineValueProvider(string commandLine)
		{
			IEnumerable<ICommandLineElement> elements = new MonadicCommandLineParser().Parse(commandLine);

			Dictionary<string, object> dictionary = elements.Where(x => x is IDefinitionElement)
				.Cast<IDefinitionElement>()
				.Select(x => new Tuple<string, object>(x.Key, x.Value))
				.Union(elements.Where(x => x is ISwitchElement)
				       	.Cast<ISwitchElement>()
				       	.Select(x => new Tuple<string, object>(x.Key, x.Value)))
				.ToDictionary(x => x.First, x => x.Second);

			_provider = new DictionaryValueProvider(dictionary);
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction)
		{
			return _provider.GetValue(key, matchingValueAction);
		}

		public bool GetValue(string key, Func<object, bool> matchingValueAction, Action missingValueAction)
		{
			return _provider.GetValue(key, matchingValueAction, missingValueAction);
		}

		public void GetAll(Action<string, object> valueAction)
		{
			_provider.GetAll(valueAction);
		}
	}
}