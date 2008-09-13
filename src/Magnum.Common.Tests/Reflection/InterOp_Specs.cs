namespace Magnum.Common.Specs.Reflection
{
    using NUnit.Framework;

    [TestFixture]
    public class When_transmogrifying_an_object :
        SerializationSpecificationBase
    {
        private readonly Bob _bob = new Bob { Name = "Dru" };
        private readonly Order1 _order = new Order1{FirstName = "Chris", LastName = "Patterson"};
        private byte[] bytes;

        protected override void After_each_specification()
        {
            bytes = null;
        }

        [Test]
        public void Shouldnt_care_about_namespaces()
        {
            bytes = Serialize(_bob);
            var output = Deserialize<Tests.Reflection.Different.Bob>(bytes);

            Assert.AreEqual(_bob.Name, output.Name);
        }

        [Test]
        public void Shouldnt_care_about_class_names()
        {
            bytes = Serialize(_bob);
            var output = Deserialize<Bill>(bytes);

            Assert.AreEqual(_bob.Name, output.Name);
        }


        [Test]
        public void Casing_shouldnt_matter()
        {
            bytes = Serialize(_bob);
            var output = Deserialize<Jack>(bytes);

            Assert.AreEqual(_bob.Name, output.name);
        }

        [Test]
        public void Order_should_matter()
        {
            bytes = Serialize(_order);
            var output = Deserialize<Order2>(bytes);

            //because order matters
            Assert.AreNotEqual(_order.FirstName, output.FirstName);
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
}

namespace Magnum.Common.Tests.Reflection
{
    using NUnit.Framework;

    namespace Different
    {
        public class Bob
        {
            public string Name { get; set; }
        }
    }

}