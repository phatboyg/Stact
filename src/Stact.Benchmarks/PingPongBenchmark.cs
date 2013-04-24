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
namespace Stact.Benchmarks
{
    using System;
    using System.Diagnostics;


//    public class PingPongBenchmark
//    {
//        public void Run()
//        {
//            Stopwatch timer = Stopwatch.StartNew();
//
//            const int actorCount = 20;
//            const int pingCount = 4000;
//
//            var actors = new ActorRef[actorCount + 1];
//
//            var complete = new Future<int>();
//
//            var latch = new CountdownLatch(actorCount * pingCount, complete.Complete);
//
//            for (int i = actorCount; i >= 0; i--)
//            {
//                actors[i] = StatelessActor.New(inbox =>
//                    {
//                        var pong = new Pong();
//
//                        var server = actors[(i + 1)];
//
//                        inbox.Loop(loop =>
//                            {
//                                loop.Receive<Message<Ping>>(request =>
//                                    {
//                                        request.Respond(pong);
//                                        loop.Continue();
//                                    });
//                            });
//
//
//                        if (i < actorCount)
//                        {
//                            var ping = new Ping();
//                            int count = 0;
//
//                            Action loop = null;
//                            loop = () =>
//                                {
//                                    server.Request(ping, inbox)
//                                        .ReceiveResponse<Pong>(response =>
//                                            {
//                                                latch.CountDown();
//                                                count++;
//                                                if (count < pingCount)
//                                                    loop();
//                                            });
//                                };
//
//                            loop();
//                        }
//                    });
//            }
//
//            bool completed = complete.WaitUntilCompleted(5.Minutes());
//
//            timer.Stop();
//
//            for (int i = 0; i < actorCount; i++)
//            {
//                actors[i].Exit();
//                actors[i] = null;
//            }
//
//            if (!completed)
//            {
//                Console.WriteLine("Process did not complete");
//                return;
//            }
//
//            Console.WriteLine("Processed {0} messages in with {1} actors in {2}ms", pingCount, actorCount, timer.ElapsedMilliseconds);
//
//            Console.WriteLine("That's {0} messages per second!", ((long)pingCount * actorCount * 2 * 1000) / timer.ElapsedMilliseconds);
//        }
//
//
//        class Ping
//        {
//        }
//
//        class Pong
//        {
//            
//        }
//    }
}