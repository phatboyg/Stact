namespace Magnum.Specs.CEP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PatternBasedObjectParser :
        BaseObjectParser<IEnumerable<object>>
    {
        public PatternBasedObjectParser()
        {
            PossibleAttackPattern = Rep(Obj<LoginFailed>().And(Obj<LoginFailed>()));

            All = from o in AnyObject
                from a in PossibleAttackPattern
                  select new PossibleBruteForceAttack();
        }

        public IEnumerable<PossibleBruteForceAttack> Parse(IEnumerable<object> inFeed)
        {
            var outa = All(inFeed);
            while(outa != null)
            {
                yield return outa.Value;

                outa = All(outa.Rest);
            }
        }

        public Parser<IEnumerable<object>, object[]> PossibleAttackPattern;
        public Parser<IEnumerable<object>, PossibleBruteForceAttack> All;

        public override Parser<IEnumerable<object>, object> AnyObject
        {
            get { return input => input.Count() > 0 ? new Result<IEnumerable<object>, object>(input.First(), input.Skip(1).ToList()) : null; }
        }
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