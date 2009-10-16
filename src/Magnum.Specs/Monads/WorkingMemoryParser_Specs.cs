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
namespace Magnum.Specs.Monads
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using Magnum.Monads.Parser;
	using NUnit.Framework;

	[TestFixture]
	public class Parsing_a_working_memory_of_objects
	{
		[Test]
		public void Should_match_each_instance_of_an_object()
		{
			var workingMemory = new HashSet<object>();
			workingMemory.Add(new object());
			workingMemory.Add(new Claim());

			var parser = new EnumerableObjectParser();

			int count = 0;
			int total = 0;
			Result<IEnumerable<object>, object> result = parser.All(workingMemory);
            while (result != null)
            {
				if (result.Value.GetType() == typeof(Claim))
				{
					count++;
				}

            	total++;

            	result = parser.All(result.Rest);
            }

			count.ShouldEqual(1);
			total.ShouldEqual(2);
		}
	}

	public abstract class AbstractObjectParser<TInput> :
		AbstractParser<TInput>
	{
		protected abstract Parser<TInput, object> AnyObject { get; }

		public Parser<TInput, object> Obj(Predicate<object> filter)
		{
			return from obj in AnyObject
			       where filter(obj)
			       select obj;
		}

		public Parser<TInput, object> Obj<TResult>()
		{
			return from obj in AnyObject
			       where typeof (TResult).IsAssignableFrom(obj.GetType())
			       select obj;
		}
	}

	public abstract class WorkingMemoryObjectParser<TInput> :
		AbstractObjectParser<TInput>
	{
		protected WorkingMemoryObjectParser()
		{
			Claim = from c in Obj<Claim>()
			        select c as Claim;

			All = (from c in Claim select (object)c)
				.Or(from ignore in Obj<object>() select ignore);
		}

		public Parser<TInput, Claim> Claim { get; private set; }
		public Parser<TInput, object> All { get; private set; }
	}

	public class EnumerableObjectParser :
		WorkingMemoryObjectParser<IEnumerable<object>>
	{
		protected override Parser<IEnumerable<object>, object> AnyObject
		{
			get
			{
				return input =>
					{
						object obj = input.FirstOrDefault();
						if(obj == default(object))
							return null;

						return new Result<IEnumerable<object>, object>(obj, input.Skip(1));
					};
			}
		}
	}

	public class Claim
	{
		public string Name { get; set; }
	}
}