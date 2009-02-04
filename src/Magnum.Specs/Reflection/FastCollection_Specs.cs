namespace Magnum.Common.Specs.Reflection
{
    using System.Collections;
    using System.Collections.Generic;
    using Common.Reflection;
    using MbUnit.Framework;

    [TestFixture]
    public class FastCollection_Specs
    {
        [Test]
        public void Generic_Add()
        {
            IList<int> b = new List<int>();

            var fc = new FastCollection<IList<int>, int>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);
        }

        [Test]
        public void Generic_Remove()
        {
            IList<int> b = new List<int>();

            var fc = new FastCollection<IList<int>, int>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);

            fc.Remove(b, 2);

            Assert.AreEqual(0, b.Count);
        }

        [Test]
        public void Add()
        {
            IList b = new List<int>();

            var fc = new FastCollection<IList>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);
        }

        [Test]
        public void Remove()
        {
            IList b = new List<int>();

            var fc = new FastCollection<IList>();

            fc.Add(b, 2);

            Assert.AreEqual(1, b.Count);

            fc.Remove(b, 2);

            Assert.AreEqual(0, b.Count);
        }
    }
}