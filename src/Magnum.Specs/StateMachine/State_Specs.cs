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
    using NUnit.Framework;

    [TestFixture]
    public class State_Specs
    {
        [Test]
        public void States_should_automatically_be_created_for_the_class()
        {
            ExampleStateMachine example = new ExampleStateMachine();

            Assert.IsNotNull(ExampleStateMachine.Initial);

            Assert.AreEqual(ExampleStateMachine.Initial.Name, "Initial");
        }

        [Test]
        public void The_initial_state_should_be_set()
        {
            ExampleStateMachine example = new ExampleStateMachine();

            Assert.AreEqual(ExampleStateMachine.Initial, example.CurrentState);
        }

        [Test]
        public void The_transitions_should_work()
        {
            ExampleStateMachine example = new ExampleStateMachine();

            example.SubmitOrder();

            Assert.AreEqual(ExampleStateMachine.WaitingForPayment, example.CurrentState);

            example.SubmitPayment();

            Assert.AreEqual(ExampleStateMachine.WaitingForPaymentApproval, example.CurrentState);

            example.ApprovePayment();

            Assert.AreEqual(ExampleStateMachine.Completed, example.CurrentState);
        }

        [Test]
        public void Typed_events_should_carry_their_data_to_the_expression()
        {
            ExampleStateMachine example = new ExampleStateMachine();

            example.SubmitCommentCard(new CommentCard { IsComplaint = true });

            Assert.AreEqual(ExampleStateMachine.WaitingForManager, example.CurrentState);
        }

        [Test]
        public void Multiple_expressions_per_event_should_run_in_order()
        {
            ExampleStateMachine example = new ExampleStateMachine();

            example.SubmitCommentCard(new CommentCard { IsComplaint = true });

            Assert.AreEqual(ExampleStateMachine.WaitingForManager, example.CurrentState);

            example.BurnCommentCard();

            Assert.AreEqual(ExampleStateMachine.Completed, example.CurrentState);
        }

        [Test]
        public void Typed_events_should_carry_their_data_to_the_expression_other()
        {
            ExampleStateMachine example = new ExampleStateMachine();

            example.SubmitCommentCard(new CommentCard { IsComplaint = false });

            Assert.AreEqual(ExampleStateMachine.Completed, example.CurrentState);
        }

    }
}