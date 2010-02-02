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

	public abstract class CommandLineElementParser<TResult> :
		AbstractParser<IEnumerable<ICommandLineElement>>
	{
		public Parser<IEnumerable<ICommandLineElement>, ICommandLineElement> AnyElement
		{
			get { return input => input.Any() ? new Result<IEnumerable<ICommandLineElement>, ICommandLineElement>(input.First(), input.Skip(1)) : null; }
		}

		public abstract Parser<IEnumerable<ICommandLineElement>, TResult> All { get; }

		public IEnumerable<TResult> Parse(IEnumerable<ICommandLineElement> elements)
		{
			Result<IEnumerable<ICommandLineElement>, TResult> result = All(elements);
			while (result != null)
			{
				yield return result.Value;

				result = All(result.Rest);
			}
		}

		public Parser<IEnumerable<ICommandLineElement>, ArgumentElement> Argument()
		{
			return from c in AnyElement
			       where c.GetType() == typeof (ArgumentElement)
			       select (ArgumentElement) c;
		}

		public Parser<IEnumerable<ICommandLineElement>, ArgumentElement> Argument(string value)
		{
			return from c in AnyElement
			       where c.GetType() == typeof (ArgumentElement)
			       where ((ArgumentElement) c).Id == value
			       select (ArgumentElement) c;
		}

		public Parser<IEnumerable<ICommandLineElement>, ArgumentElement> Argument(Predicate<ArgumentElement> pred)
		{
			return from c in AnyElement
			       where c.GetType() == typeof (ArgumentElement)
			       where pred((ArgumentElement) c)
			       select (ArgumentElement) c;
		}
	}
}