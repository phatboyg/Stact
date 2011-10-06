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
    using Magnum.Extensions;

	/// <summary>
	/// <para>Write a ring benchmark. Create N processes in a ring. Send a message round the ring M times. So that a total of N * M messages get sent. Time how long this takes for different values of N and M.</para>
	/// <para>
	/// More info:
	/// <list type="bullet">
	/// <value>http://erl.nfshost.com/2007/07/28/a-ring-benchmark-in-erlang/</value>
	/// <value>http://www.rodenas.org/blog/2007/08/27/erlang-ring-problem/</value>
	/// <value>http://muharem.wordpress.com/2007/07/31/erlang-vs-stackless-python-a-first-benchmark/</value>
	/// </list>
	/// </para>
	/// </summary>
    public class RingBenchmark
    {
        static Future<bool> _complete;
        static ActorFactory<RingNode> _ringNodeFactory;
        ActorInstance _first;

        public void Run()
        {
            _ringNodeFactory = ActorFactory.Create(inbox => new RingNode(inbox));

            Stopwatch timer = Stopwatch.StartNew();

            int nodeCount = 10000;
            int roundCount = 10000;

            Run(nodeCount, roundCount);

            timer.Stop();

            Console.WriteLine("Ring Benchmark");

            Console.WriteLine("Processed {0} rings with {1} nodes in {2}ms", roundCount, nodeCount,
                              timer.ElapsedMilliseconds);

            Console.WriteLine("That's {0} messages per second!",
                              ((long)nodeCount*roundCount*1000)/timer.ElapsedMilliseconds);
        }

        public void Run(int nodeCount, int roundCount)
        {
            _complete = new Future<bool>();
            _first = _ringNodeFactory.GetActor();
            _first.Request(new Init
                {
                    NodeCount = nodeCount - 1,
                    RoundCount = roundCount,
                }, _first);

            _complete.WaitUntilCompleted(120.Seconds());
        }


        class Init
        {
            public int NodeCount { get; set; }
            public int RoundCount { get; set; }
        }


        class RingNode :
            Actor
        {
            public RingNode(Inbox inbox)
            {
                inbox.Receive<Request<Init>>(init =>
                    {
                        if (init.Body.NodeCount == 0)
                        {
                            init.Respond(new Token
                                {
                                    RemainingRounds = init.Body.RoundCount
                                });

                            inbox.Loop(loop =>
                                {
                                    loop.Receive<Token>(token =>
                                        {
                                            int remaining = token.RemainingRounds - 1;

                                            if (remaining == 0)
                                                _complete.Complete(true);
                                            else
                                            {
                                                init.Respond(new Token
                                                    {
                                                        RemainingRounds = remaining,
                                                    });

                                                loop.Continue();
                                            }
                                        });
                                });
                        }
                        else
                        {
                            ActorInstance next = _ringNodeFactory.GetActor();
                            next.Request(new Init
                                {
                                    NodeCount = init.Body.NodeCount - 1,
                                    RoundCount = init.Body.RoundCount,
                                }, init.ResponseChannel);

                            inbox.Loop(loop =>
                                {
                                    loop.Receive<Token>(token =>
                                        {
                                            next.Send(token);
                                            loop.Continue();
                                        });
                                });
                        }
                    });
            }
        }


        class Token
        {
            public int RemainingRounds { get; set; }
        }
    }
}