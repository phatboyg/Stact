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
namespace Stact.Routing
{
    using Configuration;


    public static class RoutingEngineConfiguratorExtensions
    {
        public static RemoveActivation Receive<T>(this RoutingEngineConfigurator configurator,
                                                  SenderConsumer<T> consumer)
        {
            return configurator.Receive((Message<T> message) => consumer(message.Sender, message.Body));
        }

        public static RemoveActivation ReceiveBody<T>(this RoutingEngineConfigurator configurator, Consumer<T> consumer)
        {
            return configurator.Receive((Message<T> message) => consumer(message.Body));
        }

        public static RemoveActivation ReceiveBody<T>(this RoutingEngineConfigurator configurator,
                                                      SelectiveConsumer<T> consumer)
        {
            return configurator.SelectiveReceive((Message<T> message) =>
                {
                    Consumer<T> accepted = consumer(message.Body);
                    if (accepted == null)
                        return null;

                    return x => accepted(x.Body);
                });
        }
    }
}