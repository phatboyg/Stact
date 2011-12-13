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
namespace Stact.Actors.Actors
{
    using System;
    using System.Collections.Generic;


    public class SupervisorActor
    {
        readonly UntypedActor _inbox;

        public SupervisorActor(UntypedActor inbox, ActorExceptionHandler handler,
                               Action<ActorRef, ActorRestartLimitReached> restartLimitReachedHandler)
        {
            _inbox = inbox;
            //inbox.SetExceptionHandler(handler);

            //inbox.Receive<ActorRestartLimitReached>(msg => restartLimitReachedHandler(inbox, msg));
        }

        public void HandleStop(Message<Stop> request)
        {
//            IEnumerator<ActorRef> next = _inbox.LinkedActors.GetEnumerator();
//            if (next.MoveNext())
//            {
//                StatelessActor.New(x => x.Loop(loop =>
//                    {
//                        next.Current.Request<Stop>(x)
//                            .Receive<Message<Stop>>(response =>
//                                {
//                                    if (next.MoveNext())
//                                        loop.Continue();
//                                    else
//                                        request.Respond<Stop>();
//                                });
//                    }));
//            }
        }
    }
}