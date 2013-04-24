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
namespace Stact
{
    using System;
    using Internal;


    public static class ExtensionsToActorInstance
    {
        public static IDisposable ExitOnDispose(this ActorRef actor)
        {
            return new DisposeCallback(() => actor.BlockingRequest<Exit>(TimeSpan.FromSeconds(60)));
        }

        public static IDisposable ExitOnDispose(this ActorRef actor, TimeSpan timeout)
        {
            return new DisposeCallback(() => actor.BlockingRequest<Exit>(timeout));
        }

        public static bool BlockingRequest<TRequest>(this ActorRef actor, TRequest request, TimeSpan timeout)
        {
            var future = new Future<Message<TRequest>>();

            ActorRef responseActor =
                StatelessActor.New(x => x.Receive<TRequest>(message => future.Complete(message)));

            actor.Request(request, responseActor);

            return future.WaitUntilCompleted(timeout);
        }

        public static bool BlockingRequest<TRequest>(this ActorRef actor, TimeSpan timeout)
        {
            var future = new Future<Message<TRequest>>();

            ActorRef responseActor =
                StatelessActor.New(x => x.Receive<TRequest>(message => future.Complete(message)));

            actor.Request<TRequest>(responseActor);

            return future.WaitUntilCompleted(timeout);
        }
    }
}