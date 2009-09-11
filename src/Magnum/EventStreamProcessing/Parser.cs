using System;

namespace Magnum.EventStreamProcessing
{
    public delegate Result<TInputStream, TParsedValue> Parser<TInputStream, TParsedValue>(TInputStream input);

    public static class ParserCombinatorExtensions
    {
        public static Parser<TInputStream, TParsedValue> Or<TInputStream, TParsedValue>(
            this Parser<TInputStream, TParsedValue> parser1,
            Parser<TInputStream, TParsedValue> parser2)
        {
            return input => parser1(input) ?? parser2(input);
        }
        public static Parser<TInputStream, TParsedValue2> And<TInputStream, TParsedValue1, TParsedValue2>(
            this Parser<TInputStream, TParsedValue1> parser1,
            Parser<TInputStream, TParsedValue2> parser2)
        {
            return input => parser2(parser1(input).Rest);
        }
    }

    public static class ParserCombinatorsMonad
    {
        // By providing Select, Where and SelectMany methods on Parser<TInput,TValue> we make the 
        // C# Query Expression syntax available for manipulating Parsers.  
        public static Parser<TInputStream, TParsedValue> Where<TInputStream, TParsedValue>(
            this Parser<TInputStream, TParsedValue> parser,
            Func<TParsedValue, bool> pred)
        {
            return input =>
                       {
                           var res = parser(input);
                           if (res == null || !pred(res.Value)) return null;
                           return res;
                       };
        }
        public static Parser<TInputStream, TParsedValue2> Select<TInputStream, TParsedValue, TParsedValue2>(
            this Parser<TInputStream, TParsedValue> parser,
            Func<TParsedValue, TParsedValue2> selector)
        {
            return input =>
                       {
                           var res = parser(input);
                           if (res == null) return null;
                           return new Result<TInputStream, TParsedValue2>(selector(res.Value), res.Rest);
                       };
        }
        public static Parser<TInputStream, TParsedValue2> SelectMany<TInputStream, TParsedValue, TIntermediate, TParsedValue2>(
            this Parser<TInputStream, TParsedValue> parser,
            Func<TParsedValue, Parser<TInputStream, TIntermediate>> selector,
            Func<TParsedValue, TIntermediate, TParsedValue2> projector)
        {
            return input =>
                       {
                           var res = parser(input);
                           if (res == null) return null;
                           var val = res.Value;
                           var res2 = selector(val)(res.Rest);
                           if (res2 == null) return null;
                           return new Result<TInputStream, TParsedValue2>(projector(val, res2.Value), res2.Rest);
                       };
        }
    }
}