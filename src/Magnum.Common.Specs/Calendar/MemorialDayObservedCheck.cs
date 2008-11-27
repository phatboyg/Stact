namespace Magnum.Common.Specs.Calendar
{
    using System;
    using Common.Calendar.Holidays;
    using MbUnit.Framework;

    [TestFixture]
    public class MemorialDayObservedCheckTest
    {
        [Test]
        public void Bob()
        {
            var c = new MemorialDayObservedCheck();
            Assert.IsTrue(c.Check(new DateTime(2008, 5, 26)));
        }
    }
}