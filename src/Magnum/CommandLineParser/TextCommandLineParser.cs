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
    using System.Linq;
    using Monads.Parser;

    public abstract class TextCommandLineParser<TInput> :
        AbstractCharacterParser<TInput>
    {
        protected TextCommandLineParser()
        {
            Whitespace = Rep(Char(' ').Or(Char('\t').Or(Char('\n')).Or(Char('\r'))));
            NewLine = Rep(Char('\r').Or(Char('\n')));

            Id = from w in Whitespace
                 from c in Char(char.IsLetter)
                 from cs in Rep(Char(char.IsLetterOrDigit))
                 select cs.Aggregate(c.ToString(), (s, ch) => s + ch);

            Argument = (from c in Char('a') select (ICommandLineElement) new SwitchElement(c))
                .Or(from c in Char('b') select (ICommandLineElement) new SwitchElement(c));

            Definition = (from w in Whitespace
                          from c in Char('-').Or(Char('/'))
                          from key in Id
                          from eq in Char(':').Or(Char('='))
                          from value in Value
                          select (ICommandLineElement) new DefinitionElement(key, value))
                .Or(from w in Whitespace
                    from c in Char('-').Or(Char('/'))
                    from key in Id
                    from ws in Whitespace
                    from oq in Char('"')
                    from value in Rep(Char(x => x != '"'))
                    from cq in Char('"')
                    select (ICommandLineElement) new DefinitionElement(key, value.Aggregate("", (s, ch) => s + ch)));

            Value = (from symbol in Rep(Char(char.IsLetterOrDigit).Or(Char(char.IsPunctuation))) select symbol.Aggregate("", (s, ch) => s + ch));

            Switch = from w in Whitespace
                     from c in Char('-').Or(Char('/'))
                     from arg in Argument
                     select arg;

            Token = from w in Whitespace
                    from o in Char('[')
                    from t in Id
                    from c in Char(']')
                    select t;

            Element = (from id in Id select (ICommandLineElement) new ArgumentElement(id))
                .Or(from token in Token select (ICommandLineElement) new TokenElement(token))
                .Or(from element in Definition select element)
                .Or(from element in Switch select element);


            All = from t in Element select t;
        }

        public Parser<TInput, ICommandLineElement> Definition { get; set; }

        public Parser<TInput, string> Id { get; private set; }
        public Parser<TInput, string> Token { get; private set; }
        public Parser<TInput, string> Value { get; private set; }

        public Parser<TInput, ICommandLineElement> Switch { get; private set; }
        public Parser<TInput, char[]> Whitespace { get; private set; }
        public Parser<TInput, char[]> NewLine { get; private set; }

        public Parser<TInput, ICommandLineElement> Argument { get; private set; }

        public Parser<TInput, ICommandLineElement> Element { get; private set; }
        public Parser<TInput, ICommandLineElement> All { get; private set; }
    }
}