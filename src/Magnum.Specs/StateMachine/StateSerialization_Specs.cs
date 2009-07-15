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
namespace Magnum.Specs.StateMachine
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using NUnit.Framework;

    [TestFixture]
    public class StateSerialization_Specs
    {
        private BinaryFormatter _formatter;

        [SetUp]
        public void Setup()
        {
            _formatter = new BinaryFormatter();
        }

        [TearDown]
        public void Teardown()
        {
            _formatter = null;
        }

        [Test]
        public void Serializing_a_state_machine_should_restore_properly()
        {
            ExampleStateMachine example = new ExampleStateMachine();

            example.SubmitOrder();

            byte[] data;
            using (MemoryStream output = new MemoryStream())
            {
                _formatter.Serialize(output, example);
                data = output.ToArray();
            }

            using (MemoryStream input = new MemoryStream(data))
            {
                var copied = (ExampleStateMachine) _formatter.Deserialize(input);

                Assert.AreEqual(ExampleStateMachine.WaitingForPayment, copied.CurrentState);
            }
        }
    }
}