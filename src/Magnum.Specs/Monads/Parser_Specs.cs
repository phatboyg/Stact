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
	using System.Diagnostics;
	using System.Linq;
	using MbUnit.Framework;

	[TestFixture]
	public class Parser_Specs
	{
		[Test]
		public void Walking_like_an_egyptian()
		{
			MiniMLParserFromString parser = new MiniMLParserFromString();
			Result<string, Term> result =
				parser.All(@"let true = \x.\y.x in 
                         let false = \x.\y.y in 
                         let if = \b.\l.\r.(b l) r in
                         if true then false else true;");
		}


		[Test]
		public void Ripping_one_from_JSON_should_be_cool()
		{
			StringJsonParser parser = new StringJsonParser();

			var result = parser.All(@"name = 'hello';");

			Trace.WriteLine(result.Rest);
			Trace.WriteLine(result.Value);
			
		}

	}


	public abstract class JsonParser<TInput> :
		ParserCombinatorsMonad.CharParsers<TInput>
	{
		protected JsonParser()
		{
			Whitespace = Rep(Char(' ').Or(Char('\t').Or(Char('\n')).Or(Char('\r'))));

			Id = from w in Whitespace
				 from c in Char(char.IsLetter)
				 from cs in Rep(Char(char.IsLetterOrDigit))
				 select cs.Aggregate(c.ToString(), (acc, ch) => acc + ch);

			String = from w in Whitespace
			         from c in Char('\'')
			         from cs in Rep(Char(char.IsLetterOrDigit).Or(Char(' ')))
					 from x in Char('\'')
			         select cs.Aggregate(c.ToString(), (acc, ch) => acc + ch);

			WsChr = chr => Whitespace.And(Char(chr));


			Assignment = from x in Id
			              from u1 in WsChr('=')
			              from u2 in String
			              select (Term) new LetTerm(x, null, null);


			All = from t in Assignment from u in WsChr(';') select t;
		}

		public Parser<TInput, string> Id { get; private set; }
		public Parser<TInput, string> String { get; private set; }
		public Parser<TInput, Term> Assignment { get; private set; }

		public Parser<TInput, char[]> Whitespace { get; private set; }
		public Func<char, Parser<TInput, char>> WsChr {get; private set;}

		public Parser<TInput, Term> All { get; private set; }
	}

	public class StringJsonParser : JsonParser<string>
	{
		public override Parser<string, char> AnyChar
		{
			get { { return input => input.Length > 0 ? new Result<string, char>(input[0], input.Substring(1)) : null; } }
		}
	}



	public class Result<TInput, TValue>
	{
		public Result(TValue value, TInput rest)
		{
			Value = value;
			Rest = rest;
		}

		public TValue Value { get; private set; }
		public TInput Rest { get; private set; }
	}

	public delegate Result<TInput, TValue> Parser<TInput, TValue>(TInput input);

	public static class ParserCombinatorExtensions
	{
		public static Parser<TInput, TValue> Or<TInput, TValue>(this Parser<TInput, TValue> parser1, Parser<TInput, TValue> parser2)
		{
			return input => parser1(input) ?? parser2(input);
		}

		public static Parser<TInput, TValue2> And<TInput, TValue1, TValue2>(this Parser<TInput, TValue1> parser1, Parser<TInput, TValue2> parser2)
		{
			return input => parser2(parser1(input).Rest);
		}
	}

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

		public abstract class Parsers<TInput>
		{
			public Parser<TInput, TValue> Succeed<TValue>(TValue value)
			{
				return input => new Result<TInput, TValue>(value, input);
			}

			public Parser<TInput, TValue[]> Rep<TValue>(Parser<TInput, TValue> parser)
			{
				return Rep1(parser).Or(Succeed(new TValue[0]));
			}

			public Parser<TInput, TValue[]> Rep1<TValue>(Parser<TInput, TValue> parser)
			{
				return from x in parser
					   from xs in Rep(parser)
					   select (new[] { x }).Concat(xs).ToArray();
			}
		}

		public abstract class CharParsers<TInput> : Parsers<TInput>
		{
			public abstract Parser<TInput, char> AnyChar { get; }

			public Parser<TInput, char> Char(char ch)
			{
				return from c in AnyChar where c == ch select c;
			}

			public Parser<TInput, char> Char(Predicate<char> pred)
			{
				return from c in AnyChar where pred(c) select c;
			}
		}
	}
	// Term and its derived classes define the AST for terms in the MiniML language.
	public abstract class Term { }
	public class LambdaTerm : Term { public readonly string Ident; public readonly Term Term; public LambdaTerm(string i, Term t) { Ident = i; Term = t; } }
	public class LetTerm : Term { public readonly string Ident; public readonly Term Rhs; public Term Body; public LetTerm(string i, Term r, Term b) { Ident = i; Rhs = r; Body = b; } }
	public class AppTerm : Term { public readonly Term Func; public readonly Term[] Args; public AppTerm(Term func, Term[] args) { Func = func; Args = args; } }
	public class VarTerm : Term { public readonly string Ident; public VarTerm(string ident) { Ident = ident; } }
	// Provides a set of parsers for the MiniML Language defined above.  
public abstract class MiniMLParsers<TInput> : ParserCombinatorsMonad.CharParsers<TInput>{
    public MiniMLParsers() {
		Whitespace = Rep(Char(' ').Or(Char('\t').Or(Char('\n')).Or(Char('\r'))));
        WsChr =  chr => Whitespace.And(Char(chr));
        Id =     from w in Whitespace
                 from c in Char(char.IsLetter)
                 from cs in Rep(Char(char.IsLetterOrDigit))
                 select cs.Aggregate(c.ToString(),(acc,ch) => acc+ch);
        Ident =  from s in Id where s != "let" && s != "in" select s;
        LetId =  from s in Id where s == "let" select s;
        InId =   from s in Id where s == "in" select s;
        Term1 = (from x in Ident 
                 select (Term)new VarTerm(x))
				.Or(
                (from u1 in WsChr('(') 
                 from t in Term 
                 from u2 in WsChr(')') 
                 select t));
        Term =  (from u1 in WsChr('\\')
                 from x in Ident
                 from u2 in WsChr('.')
                 from t in Term
                 select (Term)new LambdaTerm(x,t))
				.Or(
                (from letid in LetId
                 from x in Ident
                 from u1 in WsChr('=')
                 from t in Term
                 from inid in InId
                 from c in Term
                 select (Term)new LetTerm(x,t,c)))
				.Or(
                (from t in Term1
                 from ts in Rep(Term1)
                 select (Term)new AppTerm(t,ts)));
        All =    from t in Term from u in WsChr(';') select t;
    }
	public Parser<TInput, char[]> Whitespace;
	public Func<char, Parser<TInput, char>> WsChr;
	public Parser<TInput, string> Id;
	public Parser<TInput, string> Ident;
	public Parser<TInput, string> LetId;
	public Parser<TInput, string> InId;
	public Parser<TInput, Term> Term;
	public Parser<TInput, Term> Term1;
	public Parser<TInput, Term> All;
}

public class MiniMLParserFromString : MiniMLParsers<string>
{
	public override Parser<string, char> AnyChar
	{
		get { { return input => input.Length > 0 ? new Result<string, char>(input[0], input.Substring(1)) : null; } }
	}
}
}