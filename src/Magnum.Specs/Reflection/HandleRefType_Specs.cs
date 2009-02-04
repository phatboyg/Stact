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