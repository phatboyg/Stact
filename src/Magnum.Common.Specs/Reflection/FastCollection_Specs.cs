namespace Magnum.Common.Specs.Reflection
{
    using System.Collections.Generic;
    using System.Reflection;
    using Common.Reflection;
    using MbUnit.Framework;

    [TestFixture]
    public class FastCollection_Specs
    {
        [Test]
        public void Add()
        {
            IList<int> b = new List<int>();
            
            PropertyInfo pi = typeof (TestClass).GetProperty("Numbers");
            FastCollection<TestClass, IList<int>, int> fc = new FastCollection<TestClass, IList<int>, int>(pi);

            fc.AddDelegate(b, 2);

            Assert.AreEqual(1, b.Count);
        }

        [Test]
        public void Remove()
        {
            IList<int> b = new List<int>();

            PropertyInfo pi = typeof(TestClass).GetProperty("Numbers");
            FastCollection<TestClass, IList<int>, int> fc = new FastCollection<TestClass, IList<int>, int>(pi);

            fc.AddDelegate(b, 2);

            Assert.AreEqual(1, b.Count);

            fc.RemoveDelegate(b, 2);

            Assert.AreEqual(0, b.Count);
        }
        
        public class TestClass
        {
            public IList<int> Numbers { get; set; }

            public TestClass()
            {
                Numbers = new List<int>();
            }
        }
    }

    
}