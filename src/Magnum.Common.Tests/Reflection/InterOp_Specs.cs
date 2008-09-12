namespace Magnum.Common.Tests.Reflection
{
    using System.Diagnostics;
    using NUnit.Framework;

    [TestFixture]
    public class InterOp_Specs :
        SerializationSpecificationBase
    {
        private readonly Bob bob = new Bob{Name="Dru"};

        

        [Test]
        public void Shouldnt_care_about_namespaces()
        {
            byte[] bytes;
            bytes = Serialize(bob);
            var output = Deserialize<Different.Bob>(bytes);

            Assert.AreEqual(bob.Name, output.Name);
        }

        [Test]
        public void Shouldnt_care_about_class_names()
        {
            byte[] bytes;
            bytes = Serialize(bob);
            var output = Deserialize<Bill>(bytes);

            Assert.AreEqual(bob.Name, output.Name);
        }


        [Test]
        public void Shouldnt_care_about_casing()
        {
            byte[] bytes;
            bytes = Serialize(bob);
            var output = Deserialize<Jack>(bytes);

            Assert.AreEqual(bob.Name, output.name);
        }
    }

    public class Bob
    {
        public string Name { get; set; }
    }

    public class Bill
    {
        public string Name { get; set; }
    }

    public class Jack
    {
        public string name { get; set; }
    }

    namespace Different
    {
        public class Bob
        {
            public string Name { get; set; }
        }
    }
}