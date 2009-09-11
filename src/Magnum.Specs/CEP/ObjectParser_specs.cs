using System;

namespace Magnum.Specs.CEP
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using System.Linq;

    [TestFixture]
    public class ObjectParser_specs
    {
        [Test]
        public void Three_failed_attempts_in_a_row_should_fire_BruteForceMessage()
        {
            var list = new List<object>
                       {
                           new LoginSucceeded("a"),
                           new LoginFailed("b"),
                           new LoginFailed("a"),
                           new LoginFailed("b")
                       };

            var parser = new PossibleAttackPattern();

            var output = parser.Match(list);
           

            Assert.AreEqual(1, output.Count());
    
        }
    }
}