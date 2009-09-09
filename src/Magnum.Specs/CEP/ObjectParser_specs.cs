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
                           new LoginFailed(),
                           new LoginFailed(),
                           new LoginFailed()
                       };

            var parser = new PatternBasedObjectParser();

            var output = parser.Parse(list);

            Assert.AreEqual(1, output.Count());
        }
    }
}