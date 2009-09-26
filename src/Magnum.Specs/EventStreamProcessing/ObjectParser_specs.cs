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
namespace Magnum.Specs.CEP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Pipeline;
    using NUnit.Framework;

    [TestFixture]
    public class ObjectParser_specs
    {
     

        [Test]
        public void Three_failed_attempts_in_a_row_should_fire_BruteForceMessage()
        {
            var list = new List<object>
                       {
                           new LoginSucceeded("a"),
                           new LoginFailed("b"),
                           new LoginFailed("b"),
                           new LoginFailed("b")
                       };

            var parser = new PossibleAttackPattern();

            IEnumerable<PossibleBruteForceAttack> output = parser.Match(list);


            Assert.AreEqual(1, output.Count());
        }

        [Test]
        [Ignore("argh need more work")]
        public void Three_failed_attempts_in_a_row_should_fire_BruteForceMessage_with_other_user_interuption()
        {
            var list = new List<LoginAttempt>
                       {
                           new LoginFailed("b"),
                           new LoginFailed("b"),
                           new LoginSucceeded("a"),
                           new LoginFailed("b")
                       };

            var b = from a in list
                    group a by a.Username;

            var x = from a in b
                    from p in a
                    select (object)p;

            var parser = new PossibleAttackPattern();


            IEnumerable<PossibleBruteForceAttack> output = parser.Match(new List<object>(x));


            Assert.AreEqual(1, output.Count());
        }
    }
}