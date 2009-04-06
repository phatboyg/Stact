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
namespace Magnum.Specs.Serialization
{
	using System;
	using System.Linq;

	public delegate Result<TInput, TValue> Parser<TInput, TValue>(TInput input);


	public static class ParserCombinatorsMonad
	{
		public static Parser<TInput, TValue> Where<TInput, TValue>(this Parser<TInput, TValue> parser, Func<TValue, bool> pred)
		{
			return input =>
				{
					Result<TInput, TValue> result = parser(input);
					if (result == null || !pred(result.Value))
						return null;

					return result;
				};
		}

		public static Parser<TInput, TValue2> Select<TInput, TValue, TValue2>(this Parser<TInput, TValue> parser, Func<TValue, TValue2> selector)
		{
			return input =>
				{
					Result<TInput, TValue> result = parser(input);
					if (result == null)
						return null;

					return new Result<TInput, TValue2>(selector(result.Value), result.Rest);
				};
		}

		public static Parser<TInput, TValue2> SelectMany<TInput, TValue, TIntermediate, TValue2>(this Parser<TInput, TValue> parser,
																								 Func<TValue, Parser<TInput, TIntermediate>> selector,
																								 Func<TValue, TIntermediate, TValue2> projector)
		{
			return input =>
				{
					Result<TInput, TValue> result = parser(input);
					if (result == null)
						return null;

					TValue val = result.Value;
					Result<TInput, TIntermediate> nextResult = selector(val)(result.Rest);
					if (nextResult == null)
						return null;

					return new Result<TInput, TValue2>(projector(val, nextResult.Value), nextResult.Rest);
				};
		}

	}

	public abstract class Parsers<TInput>
	{
		public Parser<TInput, TValue> Succeed<TValue>(TValue value)
		{
			return input => new Result<TInput, TValue>(value, input);
		}

//		public Parser<TInput, TValue[]> Rep<TValue>(Parser<TInput, TValue> parser)
//		{
//			return Rep1(parser).Or(Succeed(new TValue[0]));
//		}
//
//		public Parser<TInput, TValue[]> Rep1<TValue>(Parser<TInput, TValue> parser)
//		{
//			return from x in parser
//				   from xs in Rep(parser)
//				   select (new[] { x }).Concat(xs).ToArray();
//		}
	}

}