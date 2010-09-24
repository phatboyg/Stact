// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.ForNHibernate.Specs.Data.Primitives
{
    using TestFramework;

    public class EqualityBehaviour_Specs
    {
        Money _primitive;
        Money _primitive2;

        [Given]
        public void Given_a_money_object_and_another_instance_of_the_same_value()
        {
            _primitive = new Money(2);
            _primitive2 = new Money(2);
        }

        [Then]
        public void Then_the_two_should_be_equal()
        {
            _primitive.ShouldEqual(_primitive2);
        }
    }

    public class InequalityBehaviour_Specs
    {
        Money _primitive;
        Money _primitive2;

        [Given]
        public void Given_a_money_object_and_another_instance_of_different_values()
        {
            _primitive = new Money(1);
            _primitive2 = new Money(2);
        }

        [Then]
        public void Then_the_two_should_be_equal()
        {
            _primitive.ShouldNotEqual(_primitive2);
        }
    }

    public class Primitive_without_an_equality_override_wont_equal
    {
        MoneyWithNoEqualsOverrides _primitive;
        MoneyWithNoEqualsOverrides _primitive2;

        [Given]
        public void Given_a_money_object_and_another_instance_of_the_same_value()
        {
            _primitive = new MoneyWithNoEqualsOverrides(2);
            _primitive2 = new MoneyWithNoEqualsOverrides(2);
        }

        [Then]
        public void Then_the_two_will_not_be_equal()
        {
            _primitive.ShouldNotEqual(_primitive2);
        }
    }
}