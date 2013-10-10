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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Benchmarque;

    public interface TokenRing
    {
        void CreateRing();

    }


    class OneHundredNodeTokenRing :
        TokenRing
    {
        ActorTokenRing _ring;

        public OneHundredNodeTokenRing()
        {
            _ring = new ActorTokenRing(200);
        }

        public void CreateRing()
        {
            _ring.Create();
        }
    }


    public class TokenRingBenchmark :
        Benchmark<TokenRing>
    {
        public void WarmUp(TokenRing instance)
        {
        }

        public void Shutdown(TokenRing instance)
        {
        }

        public void Run(TokenRing instance, int iterationCount)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> Iterations { get; private set; }
    }




    public class ActorTokenRing
    {
        readonly int _tokenCount;

        public ActorTokenRing(int nodeCount)
        {
            _tokenCount = Environment.ProcessorCount * 3 / 2;
            _nodeCount = nodeCount;


        }


        public void Create()
        {
            _initial = new Future<long>();
            ActorRef first = Actor.New<NodeState>(x => x.ChangeBehavior<InitialNodeBehavior>());
            first.Request(new Init
                {
                    NodeCount = _nodeCount - 1
                }, first);

            var completed = _initial.WaitUntilCompleted(TimeSpan.FromSeconds(30));
            if (!completed)
                throw new InvalidOperationException("Timeout waiting for ring to setup");
        }

        public void Run(int roundCount)
        {
            _complete = new Future<long>();
            ActorRef first = Actor.New<NodeState>(x => x.ChangeBehavior<InitialNodeBehavior>());
            first.Request(new Init
                {
                    NodeCount = _nodeCount - 1,
                    RoundCount = _roundCount,
                }, first);

            bool completed = _complete.WaitUntilCompleted(TimeSpan.FromSeconds(60));

            _timer.Stop();

            if (!completed)
                Console.WriteLine("TEST DID NOT COMPLETE");

            if (_complete.Value != _nodeCount * _roundCount)
                Console.WriteLine("TEST DID NOT COMPLETE ALL NODES");

            Console.WriteLine("Elapsed Time: {0}ms", _timer.ElapsedMilliseconds);
            Console.WriteLine("Create Time: {0}ms", _created);
            Console.WriteLine("Messages per second: {0,-4}",
                              ((long)_tokenCount*_nodeCount*_roundCount + _nodeCount)*1000/(_timer.ElapsedMilliseconds - _created));
        }

        static Future<long> _complete;

        static Stopwatch _timer;

        static long _created;
        int _nodeCount;
        int _roundCount;
        static Future<long> _initial;


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
                RemainingRounds = new int[100];
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

                    _initial.Complete(0);

//                    _created = _timer.ElapsedMilliseconds;
//                    for (int i = 0; i < _tokenCount; i++)
//                    {
//                        message.Sender.Send(new Token
//                        {
//                            TokenId = i,
//                            Counter = 0,
//                            RemainingRounds = message.Body.RoundCount
//                        }, _actor.State.Next);
//                    }

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