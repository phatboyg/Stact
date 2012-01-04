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
namespace Stact.Specs.Example
{
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class Creating_a_ring_of_nodes_and_passing_token_around_them
    {
        [Test]
        public void Should_complete()
        {
            int nodeCount = 100;
            int roundCount = 100;

            _complete = new Future<bool>();
            ActorRef first = Actor.New<NodeState>(x => x.Apply<InitialNodeBehavior>());
            first.Request(new Init
                {
                    NodeCount = nodeCount - 1,
                    RoundCount = roundCount,
                }, first);

            _complete.WaitUntilCompleted(30.Seconds()).ShouldBeTrue();
        }

        static Future<bool> _complete;


        class Init
        {
            public int RoundCount { get; set; }
            public int NodeCount { get; set; }
        }


        class Token
        {
            public int RemainingRounds { get; set; }
        }


        class NodeState
        {
            public ActorRef Next;
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
                    ActorRef next = Actor.New<NodeState>(x => x.Apply<InitialNodeBehavior>());
                    _actor.State.Next = next;
                    next.Send(new Init
                        {
                            NodeCount = message.Body.NodeCount - 1,
                            RoundCount = message.Body.RoundCount,
                        }, message.Sender);


                    _actor.Apply<ActiveNodeBehavior>();
                }
                else
                {
                    message.Sender.Send(new Token
                        {
                            RemainingRounds = message.Body.RoundCount
                        }, message.Sender);

                    _actor.Apply<LastNodeBehavior>();
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
                _actor.State.Next.Send(message);
            }

            public void Handle(Message<Shutdown> message)
            {
                _actor.State.Next.Send(message);
                _actor.Exit();
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
                int remaining = message.Body.RemainingRounds - 1;

                if (remaining == 0)
                    message.Sender.Send(new Shutdown(), message.Sender);
                else
                {
                    message.Sender.Send(new Token
                        {
                            RemainingRounds = remaining,
                        }, message.Sender);
                }
            }

            public void Handle(Message<Shutdown> message)
            {
                _complete.Complete(true);
            }
        }


        class Shutdown
        {
        }
    }
}