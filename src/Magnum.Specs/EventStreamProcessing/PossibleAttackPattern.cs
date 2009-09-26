using System.Collections.Generic;
using System.Linq;
using Magnum.EventStreamProcessing;

namespace Magnum.Specs.CEP
{
    public class PossibleAttackPattern :
        Pattern<IEnumerable<object>, PossibleBruteForceAttack>
    {
        public PossibleAttackPattern()
        {
            //Define( ()=> {}); ????
            BadMessage = Obj<LoginFailed, LoginFailed>();

            //how to ignore the success?
            ThePattern = from a in BadMessage
                         from b in BadMessage
                         from c in BadMessage
                         where a.Username == b.Username && b.Username == c.Username
                         select new PossibleBruteForceAttack(a, b, c);

            All = from o in AnyObject
                  from a in ThePattern
                  select a;
        }
        public Parser<IEnumerable<object>, PossibleBruteForceAttack> ThePattern;
        public Parser<IEnumerable<object>, LoginFailed> BadMessage;
        public Parser<IEnumerable<object>, PossibleBruteForceAttack> All;

        public override sealed Parser<IEnumerable<object>, object> AnyObject
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
}