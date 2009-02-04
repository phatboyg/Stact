namespace Magnum.Common.Specs
{
    using System;
    using System.Collections.Generic;
    using CollectionExtensions;
    using MbUnit.Framework;

    [TestFixture]
    public class When_retrieving_an_item_from_the_dictionary
    {
        private Dictionary<int, string> _items;

        [SetUp]
        public void Arrange()
        {
            _items = new Dictionary<int, string>
                         {
                             { 0, "Zero" },
                             { 1, "One" },
                             { 2, "Two" },
                         };
        }

        [Test]
        public void Accesing_a_missing_key_should_throw_an_exception()
        {
            try
            {
                string value = _items[3];

                Assert.Fail("The accessor should have thrown an exception");
            }
            catch (Exception)
            {
            }
        }

        [Test]
        public void Accessing_a_missing_key_should_use_the_default()
        {
            string value = _items.Retrieve(3, "Three");

            Assert.AreEqual("Three", value);
            //Assert.That(value, Is.EqualTo("Three"), "The default item was not returned");

            Assert.AreEqual("Three", _items[3]);
            //Assert.That(_items[3], Is.EqualTo("Three"), "The default item was not added to the dictionary");
        }
    }
}