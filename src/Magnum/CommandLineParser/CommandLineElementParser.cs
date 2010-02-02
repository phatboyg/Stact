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
namespace Magnum.CommandLineParser
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Monads.Parser;

	public class CommandLineElementParser<TResult> :
		AbstractParser<IEnumerable<ICommandLineElement>>,
		ICommandLineElementParser<TResult>
	{
		private readonly IList<Parser<IEnumerable<ICommandLineElement>, TResult>> _parsers;

		public CommandLineElementParser()
		{
			_parsers = new List<Parser<IEnumerable<ICommandLineElement>, TResult>>();

			All = from element in _parsers.FirstMatch() select element;
		}

		public Parser<IEnumerable<ICommandLineElement>, ICommandLineElement> AnyElement
		{
			get { return input => input.Any() ? new Result<IEnumerable<ICommandLineElement>, ICommandLineElement>(input.First(), input.Skip(1)) : null; }
		}

		public Parser<IEnumerable<ICommandLineElement>, TResult> All { get; protected set; }

		public void Add(Parser<IEnumerable<ICommandLineElement>, TResult> parser)
		{
			_parsers.Add(parser);
		}

		public Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> Definition()
		{
			return from c in AnyElement
				   where c.GetType() == typeof(DefinitionElement)
				   select (IDefinitionElement)c;
		}

		public Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> Definition(string key)
		{
			return from def in Definition()
				   where def.Key == key
				   select def;
		}

		public Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> Definitions(params string[] keys)
		{
			return from def in Definition()
				   where keys.Contains(def.Key)
				   select def;
		}

		public Parser<IEnumerable<ICommandLineElement>, ISwitchElement> Switch()
		{
			return from c in AnyElement
			       where c.GetType() == typeof (SwitchElement)
			       select (ISwitchElement) c;
		}

		public Parser<IEnumerable<ICommandLineElement>, ISwitchElement> Switch(string key)
		{
			return from sw in Switch()
			       where sw.Key == key
			       select sw;
		}

		public Parser<IEnumerable<ICommandLineElement>, ISwitchElement> Switches(params string[] keys)
		{
			return from sw in Switch()
			       where keys.Contains(sw.Key)
			       select sw;
		}

		public Parser<IEnumerable<ICommandLineElement>, IArgumentElement> Argument()
		{
			return from c in AnyElement
			       where c.GetType() == typeof (ArgumentElement)
			       select (IArgumentElement) c;
		}

		public Parser<IEnumerable<ICommandLineElement>, IArgumentElement> Argument(string value)
		{
			return from arg in Argument()
			       where arg.Id == value
			       select arg;
		}

		public Parser<IEnumerable<ICommandLineElement>, IArgumentElement> Argument(Predicate<IArgumentElement> pred)
		{
			return from arg in Argument()
			       where pred(arg)
			       select arg;
		}

		public IEnumerable<TResult> Parse(IEnumerable<ICommandLineElement> elements)
		{
			Result<IEnumerable<ICommandLineElement>, TResult> result = All(elements);
			while (result != null)
			{
				yield return result.Value;

				result = All(result.Rest);
			}
		}
	}

	public static class ExtensionForCommandLineElementParsers
	{
		public static Parser<IEnumerable<ICommandLineElement>, ISwitchElement> Optional(this Parser<IEnumerable<ICommandLineElement>, ISwitchElement> source, string key, bool defaultValue)
		{
			return input =>
				{
					IEnumerable<ICommandLineElement> query = input
						.Where(x => x.GetType() == typeof (SwitchElement))
						.Where(x => ((SwitchElement) x).Key == key);

					if (query.Any())
						return new Result<IEnumerable<ICommandLineElement>, ISwitchElement>(query.First() as ISwitchElement, input.Except(query));

					return new Result<IEnumerable<ICommandLineElement>, ISwitchElement>(new SwitchElement(key, defaultValue), input);
				};
		}

		public static Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> Optional(this Parser<IEnumerable<ICommandLineElement>, IDefinitionElement> source, string key, string defaultValue)
		{
			return input =>
				{
					IEnumerable<ICommandLineElement> query = input
						.Where(x => x.GetType() == typeof (DefinitionElement))
						.Where(x => ((DefinitionElement) x).Key == key);

					if (query.Any())
						return new Result<IEnumerable<ICommandLineElement>, IDefinitionElement>(query.First() as IDefinitionElement, input.Except(query));

					return new Result<IEnumerable<ICommandLineElement>, IDefinitionElement>(new DefinitionElement(key, defaultValue), input);
				};
		}
	}
}