// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Magnum.Specs
{
    using System;
    using System.Collections.Generic;
    using Magnum.CollectionExtensions;
    using NUnit.Framework;

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