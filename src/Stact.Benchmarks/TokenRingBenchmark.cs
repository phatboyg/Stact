// Copyright 2010-2013 Chris Patterson
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
    using System.Linq;


    public class TokenRingBenchmark
    {
         static readonly int TokenCount = Environment.ProcessorCount * 3 / 2;

        public void Run()
        {
            int nodeCount = 200;
            int roundCount = 1000;

            Console.WriteLine("Using {0} processors, {1} Tokens", Environment.ProcessorCount, TokenCount);

            _timer = Stopwatch.StartNew();

            _complete = new Future<long>();
            ActorRef first = Actor.New<NodeState>(x => x.ChangeBehavior<InitialNodeBehavior>());
            first.Request(new Init
                {
                    NodeCount = nodeCount - 1,
                    RoundCount = roundCount,
                }, first);

            bool completed = _complete.WaitUntilCompleted(TimeSpan.FromSeconds(60));

            _timer.Stop();

            if (!completed)
                Console.WriteLine("TEST DID NOT COMPLETE");

            if (_complete.Value != nodeCount * roundCount)
                Console.WriteLine("TEST DID NOT COMPLETE ALL NODES");

            Console.WriteLine("Elapsed Time: {0}ms", _timer.ElapsedMilliseconds);
            Console.WriteLine("Create Time: {0}ms", _created);
            Console.WriteLine("Messages per second: {0,-4}",
                              ((long)TokenCount*nodeCount*roundCount + nodeCount)*1000/(_timer.ElapsedMilliseconds - _created));
        }

        static Future<long> _complete;
        static Stopwatch _timer;
        static long _created;

        class Init
        {
            public int RoundCount { get; set; }
            public int NodeCount { get; set; }
        }


        class Token
        {
            public int RemainingRounds { get; set; }
            public int Counter { get; set; }
            public int TokenId { get; set; }
        }


        class NodeState
        {
            public NodeState()
            {
                RemainingRounds = new int[TokenCount];
            }

            public ActorRef Next;
            public int[] RemainingRounds;
            public int Counter;
        }


        class InitialNodeBehavior :
            Behavior<NodeState>
        {
            readonly Actor<NodeState> _actor;

            public InitialNodeBehavior(Actor<NodeState> actor)
            {
                _actor = actor;
            }

            public void Handle(Message<Init> message)
            {
                if (message.Body.NodeCount > 0)
                {
                    ActorRef next = Actor.New<NodeState>(x => x.ChangeBehavior<InitialNodeBehavior>());
                    _actor.State.Next = next;
                    next.Send(new Init
                        {
                            NodeCount = message.Body.NodeCount - 1,
                            RoundCount = message.Body.RoundCount,
                        }, message.Sender);


                    _actor.ChangeBehavior<ActiveNodeBehavior>();
                }
                else
                {
                    _actor.State.Next = message.Sender;

                    _created = _timer.ElapsedMilliseconds;
                    for (int i = 0; i < TokenCount; i++)
                    {
                        message.Sender.Send(new Token
                        {
                            TokenId = i,
                            Counter = 0,
                            RemainingRounds = message.Body.RoundCount
                        }, _actor.State.Next);
                    }

                    _actor.ChangeBehavior<LastNodeBehavior>();
                }
            }
        }


        class ActiveNodeBehavior :
            Behavior<NodeState>
        {
            readonly Actor<NodeState> _actor;

            public ActiveNodeBehavior(Actor<NodeState> actor)
            {
                _actor = actor;
            }

            public void Handle(Message<Token> message)
            {
                _actor.State.Next.Send(new Token
                    {
                        TokenId = message.Body.TokenId,
                        Counter = message.Body.Counter + 1,
                        RemainingRounds = message.Body.RemainingRounds,
                    }, message.Sender);
            }

            public void Handle(Message<Exit> message, NextExitHandler next)
            {
                _actor.State.Next.Exit(_actor.Self);

                next(message);
            }
        }


        class LastNodeBehavior :
            Behavior<NodeState>
        {
            readonly Actor<NodeState> _actor;

            public LastNodeBehavior(Actor<NodeState> actor)
            {
                _actor = actor;
            }

            public void Handle(Message<Token> message)
            {
                _actor.State.RemainingRounds[message.Body.TokenId] = message.Body.RemainingRounds - 1;
                _actor.State.Counter = message.Body.Counter + 1;

                if (_actor.State.RemainingRounds[message.Body.TokenId] == 0)
                {
                    if(_actor.State.RemainingRounds.Sum() == 0)
                        _actor.State.Next.Exit();
                }
                else
                {
                    message.Sender.Send(new Token
                        {
                            TokenId = message.Body.TokenId,
                            Counter = message.Body.Counter + 1,
                            RemainingRounds = _actor.State.RemainingRounds[message.Body.TokenId],
                        }, _actor.State.Next);
                }
            }

            public void Handle(Message<Exit> message, NextExitHandler next)
            {
                _complete.Complete(_actor.State.Counter);

                next(message);
            }
        }
    } 
}