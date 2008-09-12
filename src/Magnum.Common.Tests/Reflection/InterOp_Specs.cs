namespace Magnum.Common.Tests.Reflection
{
    using NUnit.Framework;

    [TestFixture]
    public class InterOp_Specs :
        SerializationSpecificationBase
    {
        private readonly Bob _bob = new Bob { Name = "Dru" };
        private readonly Order1 _order = new Order1{FirstName = "Chris", LastName = "Patterson"};

        [Test]
        public void Shouldnt_care_about_namespaces()
        {
            byte[] bytes;
            bytes = Serialize(_bob);
            var output = Deserialize<Different.Bob>(bytes);

            Assert.AreEqual(_bob.Name, output.Name);
        }

        [Test]
        public void Shouldnt_care_about_class_names()
        {
            byte[] bytes;
            bytes = Serialize(_bob);
            var output = Deserialize<Bill>(bytes);

            Assert.AreEqual(_bob.Name, output.Name);
        }


        [Test]
        public void Shouldnt_care_about_casing()
        {
            byte[] bytes;
            bytes = Serialize(_bob);
            var output = Deserialize<Jack>(bytes);

            Assert.AreEqual(_bob.Name, output.name);
        }

        [Test]
        public void Shouldnt_care_about_order()
        {
            byte[] bytes;
            bytes = Serialize(_order);
            var output = Deserialize<Order2>(bytes);

            Assert.AreEqual(_order.FirstName, output.FirstName);
            Assert.AreEqual(_order.LastName, output.LastName);
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

        public class Order1
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class Order2
        {
            public string LastName { get; set; }
            public string FirstName { get; set; }
        }

    }
    namespace Different
    {
        public class Bob
        {
            public string Name { get; set; }
        }
    }

}