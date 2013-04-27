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


    /// <summary>
    /// An anonymous actor is used to declare behavior inline, rather than using a 
    /// class. 
    /// </summary>
    public static class StatelessActor
    {
        static StatelessActor()
        {
            Actor.Initialize<Stateless>(x =>
                {
                    x.HandleOnThreadPool();
                    x.UseSharedScheduler();
                });
        }

        public static ActorRef New(Action<Actor<Stateless>> initializer)
        {
            return Actor.New(new Stateless(), x => x.Internals.Fiber.Execute(() => initializer(x)));
        }


        public class Stateless
        {
        }
    }
}