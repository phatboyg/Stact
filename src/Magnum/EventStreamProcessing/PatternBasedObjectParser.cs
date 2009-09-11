using System;
using System.Collections.Generic;
using System.Linq;

namespace Magnum.EventStreamProcessing
{
    

    public abstract class Pattern<TInputStream, TParsedValue> :
        BaseObjectParser<TInputStream>

    {
        public abstract IEnumerable<TParsedValue> Match(TInputStream inFeed);
    }

    public abstract class BaseObjectParser<TInputStream> :
        BasePatternParser<TInputStream>
    {
        public abstract Parser<TInputStream, object> AnyObject { get; }

        public Parser<TInputStream, object> Obj(object obj)
        {
            return from o in AnyObject where o == obj select o;
        }
        public Parser<TInputStream, object> Obj(Predicate<object> pred)
        {
            return from o in AnyObject where pred(o) select o;
        }

        public Parser<TInputStream, object> Obj<T>()
        {
            return from o in AnyObject 
                   where o is T
                   select o;
        }

        public Parser<TInputStream, U> Obj<T,U>()
        {
            return from o in AnyObject
                   where o is T
                   select (U)o;
        }
    }

    public abstract class BasePatternParser<TInputStream>
    {
        public Parser<TInputStream, TParsedValue> Succeed<TParsedValue>(TParsedValue value)
        {
            return input => new Result<TInputStream, TParsedValue>(value, input);
        }
        public Parser<TInputStream, TParsedValue[]> Rep<TParsedValue>(Parser<TInputStream, TParsedValue> parser)
        {
            return Rep1(parser).Or(Succeed(new TParsedValue[0]));
        }
        public Parser<TInputStream, TParsedValue[]> Rep1<TParsedValue>(Parser<TInputStream, TParsedValue> parser)
        {
            return from x in parser
                   from xs in Rep(parser)
                   select (new[] { x }).Concat(xs).ToArray();
        }
    }
}