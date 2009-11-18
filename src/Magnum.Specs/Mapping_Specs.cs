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
    using NUnit.Framework;

    [TestFixture]
    public class When_mapping_an_object_to_an_object
    {
        [Test]
        public void It_should_just_work()
        {
            var map = new Mapper<SourceObject, TargetObject>();

            map.From(x => x.Id).To(y => y.CustomerId);
            map.From(x => x.Name).To(y => y.DisplayName);
            map.From(x => x.Amount).To(y => y.OrderAmount);

            SourceObject source = new SourceObject
                                      {
                                          Id = 27,
                                          Name = "Chris",
                                          Amount = 234.75m,
                                      };

            TargetObject target = map.Transform(source);

            Assert.AreEqual(source.Id, target.CustomerId);
            Assert.AreEqual(source.Name, target.DisplayName);
            Assert.AreEqual(source.Amount, target.OrderAmount);
        }

        [Test]
        public void It_should_xml()
        {
            var map = new Mapper<SourceObject, TargetObject>();

            map.From(x => x.Id).To(y => y.CustomerId);
            map.From(x => x.Name).To(y => y.DisplayName);
            map.From(x => x.Amount).To(y => y.OrderAmount);

            var xml = map.WhatAmIDoing();
            Assert.AreEqual("<transform from=\"Magnum.Specs.SourceObject\" to=\"Magnum.Specs.TargetObject\">\r\n    <map from=\"Id\" to=\"CustomerId\" />\r\n    <map from=\"Name\" to=\"DisplayName\" />\r\n    <map from=\"Amount\" to=\"OrderAmount\" />\r\n</transform>\r\n", xml);
        }

        [Test]
        public void Reusable_mapper()
        {
            var map = new ReusableMapper();

            SourceObject source = new SourceObject
                                      {
                                          Id = 27,
                                          Name = "Chris",
                                          Amount = 234.75m,
                                      };

            TargetObject target = map.Transform(source);

            Assert.AreEqual(source.Id, target.CustomerId);
            Assert.AreEqual(source.Name, target.DisplayName);
            Assert.AreEqual(source.Amount, target.OrderAmount);

            var xml = map.WhatAmIDoing();
            Assert.AreEqual("<transform from=\"Magnum.Specs.SourceObject\" to=\"Magnum.Specs.TargetObject\">\r\n    <map from=\"Id\" to=\"CustomerId\" />\r\n    <map from=\"Name\" to=\"DisplayName\" />\r\n    <map from=\"Amount\" to=\"OrderAmount\" />\r\n</transform>\r\n", xml);
        }
    }

    internal class ReusableMapper : Mapper<SourceObject, TargetObject>
    {
        public ReusableMapper()
        {
            From(x => x.Id).To(y => y.CustomerId);
            From(x => x.Name).To(y => y.DisplayName);
            From(x => x.Amount).To(y => y.OrderAmount);
        }
    }

    internal class TargetObject
    {
        public int CustomerId { get; set; }

        public string DisplayName { get; set; }

        public decimal OrderAmount { get; set; }
    }

    internal class SourceObject
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Amount { get; set; }
    }
}