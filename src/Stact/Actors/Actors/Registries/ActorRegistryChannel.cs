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
namespace Stact.Actors.Registries
{
    using System;


    public class ActorRegistryChannel :
        ActorChannel
    {
        readonly ActorRegistry _registry;

        public ActorRegistryChannel(ActorRegistry registry)
        {
            _registry = registry;
        }


        public void Send<T>(Message<T> message)
        {
            if (message.DestinationAddress == null)
            {
                Console.WriteLine("No destination");
                return;
            }

            var urn = new ActorUrn(message.DestinationAddress);
            Guid key = urn.GetId();

            _registry.Get(key, actor => actor.Send(message),
                          () => { Console.WriteLine("Actor not found: " + message.DestinationAddress); });
        }
    }
}