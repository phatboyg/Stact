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


    [Scenario]
    public class Using_an_anonymous_actor
    {
        [Then]
        public void Should_not_require_extensive_namespace_references()
        {
            var responded = new Future<MyResponse>();

            ActorRef server = AnonymousActor.New(inbox =>
                {
                    inbox.Receive<Request<MyRequest>>(request =>
                        {
                            // send our response
                            request.Respond(new MyResponse());
                        });
                });

            ActorRef client = AnonymousActor.New(inbox =>
                {
                    server.Request(new MyRequest(), inbox)
                        .Receive<Response<MyResponse>>(response => responded.Complete(response.Body));
                });

            responded.WaitUntilCompleted(2.Seconds()).ShouldBeTrue();

            server.Exit();
            client.Exit();
        }


        class MyRequest
        {
        }


        class MyResponse
        {
        }
    }
}