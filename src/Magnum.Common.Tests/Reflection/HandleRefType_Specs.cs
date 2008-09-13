namespace Magnum.Common.Specs.Reflection
{
    using NUnit.Framework;

    [TestFixture]
    public class HandleRefType_Specs :
        SerializationSpecificationBase
    {
        private Person _bob = new Person { Name = "Dru", Pet = new Dog { Name = "Roxy" } };

        [Test]
        public void Ref()
        {
            byte[] bytes;
            bytes = Serialize(_bob);
            var output = Deserialize<Person>(bytes);

            Assert.AreEqual(_bob.Pet.Name, output.Pet.Name);
        }



        public class Person
        {
            public string Name { get; set; }
            public Dog Pet { get; set; }
        }

        public class Dog
        {
            public string Name { get; set; }
        }
    }
}