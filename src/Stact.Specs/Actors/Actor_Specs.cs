// Copyright 2010 Chris Patterson
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
namespace Stact.Specs.Actors
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class A_simple_actor
    {
        [Test]
        public void Should_handle_the_request()
        {
            _futureRequest.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("No request received");
        }

        [Test]
        public void Should_handle_the_response()
        {
            _futureResponse.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("No response received");
            _futureResponse.Value.Body.Result.ShouldEqual(79);
        }

        Future<Message<Add>> _futureRequest;
        Future<Message<AddResult>> _futureResponse;

        [TestFixtureSetUp]
        public void Should_respond_to_a_simple_message()
        {
            _futureRequest = new Future<Message<Add>>();
            _futureResponse = new Future<Message<AddResult>>();

            ActorRef server = StatelessActor.New(actor =>
                {
                    actor.Receive<Add>(message =>
                        {
                            int result = message.Body.Left + message.Body.Right;

                            _futureRequest.Complete(message);

                            message.Respond(new AddResult
                                {
                                    Result = result
                                });
                        });
                });


            StatelessActor.New(actor =>
                {
                    var add = new Add
                        {
                            Left = 56,
                            Right = 23
                        };

                    server.Request(add, actor)
                        .ReceiveResponse<AddResult>(response =>
                            {
                                _futureResponse.Complete(response);
                            });
                });
        }


        public class Add
        {
            public int Left { get; set; }
            public int Right { get; set; }
        }


        public class AddResult
        {
            public int Result { get; set; }
        }
    }
}