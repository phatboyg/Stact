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
namespace Magnum.Specs.Reflection
{
    using Common.Specs;
    using MbUnit.Framework;

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
    using MbUnit.Framework;

    namespace Different
    {
        public class Bob
        {
            public string Name { get; set; }
        }
    }

}