namespace Magnum.Specs.CEP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class PossibleAttackPattern :
        Pattern<IEnumerable<object>, PossibleBruteForceAttack>
    {
        public PossibleAttackPattern()
        {
            //Define( ()=> {}); ????
            BadMessage = Obj<LoginFailed, LoginAttempt>();

            ThePattern = from a in BadMessage
                                    from b in BadMessage
                                    from c in BadMessage
                                    where a.Username == b.Username && b.Username == c.Username
                                    select new PossibleBruteForceAttack(a, b, c);

            All = from o in AnyObject
                  from a in ThePattern
                  select a;
        }
        public Parse<IEnumerable<object>, PossibleBruteForceAttack> ThePattern;
        public Parse<IEnumerable<object>, LoginAttempt> BadMessage;
        public Parse<IEnumerable<object>, PossibleBruteForceAttack> All;

        public override sealed Parse<IEnumerable<object>, object> AnyObject
        {
            get { return input => input.Any() ? new Result<IEnumerable<object>, object>(input.First(), input.Skip(1).ToList()) : null; }
        }

        public override IEnumerable<PossibleBruteForceAttack> Match(IEnumerable<object> inFeed)
        {
            var outa = All(inFeed);
            while (outa != null)
            {
                yield return outa.Value;

                outa = All(outa.Rest);
            }
        }
    }

    //framework
    public abstract class Pattern<TInputStream, TParsedValue> :
        BaseObjectParser<TInputStream>

    {
        public abstract IEnumerable<TParsedValue> Match(TInputStream inFeed);
    }


    public abstract class BaseObjectParser<TInputStream> :
        BasePatternParser<TInputStream>
    {
        public abstract Parse<TInputStream, object> AnyObject { get; }

        public Parse<TInputStream, object> Obj(object obj)
        {
            return from o in AnyObject where o == obj select o;
        }
        public Parse<TInputStream, object> Obj(Predicate<object> pred)
        {
            return from o in AnyObject where pred(o) select o;
        }

        public Parse<TInputStream, object> Obj<T>()
        {
            return from o in AnyObject 
                   where o is T
                   select o;
        }

        public Parse<TInputStream, U> Obj<T,U>()
        {
            return from o in AnyObject
                   where o is T
                   select (U)o;
        }
    }

    public abstract class BasePatternParser<TInputStream>
    {
        public Parse<TInputStream, TParsedValue> Succeed<TParsedValue>(TParsedValue value)
        {
            return input => new Result<TInputStream, TParsedValue>(value, input);
        }
        public Parse<TInputStream, TParsedValue[]> Rep<TParsedValue>(Parse<TInputStream, TParsedValue> parser)
        {
            return Rep1(parser).Or(Succeed(new TParsedValue[0]));
        }
        public Parse<TInputStream, TParsedValue[]> Rep1<TParsedValue>(Parse<TInputStream, TParsedValue> parser)
        {
            return from x in parser
                   from xs in Rep(parser)
                   select (new[] { x }).Concat(xs).ToArray();
        }
    }
}