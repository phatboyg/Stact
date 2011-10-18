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
    using System.Diagnostics;
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    public class Actor_Specs
    {
        [Test]
        public void Something()
        {
            ActorRef actor = AnonymousActor.New(inbox =>
                {
                    inbox.Loop(loop =>
                        {
                            loop.Receive<Request<Add>>(message =>
                                {
                                    int result = message.Body.Left + message.Body.Right;

                                    Trace.WriteLine("Responding with " + result + " on thread "
                                                    + Thread.CurrentThread.ManagedThreadId);

                                    message.Respond(new AddResult
                                        {
                                            Result = result
                                        });

                                    loop.Continue();
                                });


                            loop.Receive<Exit>(msg =>
                                {
                                    inbox.Exit();
                                    loop.Continue();
                                });
                        });
                });

            var future = new Future<AddResult>();

            AnonymousActor.New(inbox =>
                               actor.Request(new Add
                                   {
                                       Left = 56,
                                       Right = 23
                                   }, inbox)
                                   .Receive<AddResult>(response => future.Complete(response)));

            future.WaitUntilCompleted(8.Seconds()).ShouldBeTrue("Didn't responsd");
        }


        class Add
        {
            public int Left { get; set; }
            public int Right { get; set; }
        }


        class AddResult
        {
            public int Result { get; set; }
        }
    }
}