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
namespace Stact.Routing.Internal
{
    using Configuration;


    public class MessageConsumerNodeFactory<T> :
        ConsumerNodeFactory
    {
        public RemoveActivation Create<TMessage>(Consumer<TMessage> consumer, RoutingEngineConfigurator configurator)
        {
            var messageConsumer = consumer as Consumer<Message<T>>;
            var consumerNode = new ConsumerNode<Message<T>>(configurator.Engine, messageConsumer);

            return AddActivation(configurator, consumerNode);
        }

        public RemoveActivation Create<TMessage>(SelectiveConsumer<TMessage> consumer,
                                                 RoutingEngineConfigurator configurator)
        {
            var messageConsumer = consumer as SelectiveConsumer<Message<T>>;
            var consumerNode = new SelectiveConsumerNode<Message<T>>(configurator.Engine, messageConsumer);

            return AddActivation(configurator, consumerNode);
        }

        static RemoveActivation AddActivation(RoutingEngineConfigurator configurator,
                                              Activation<Message<T>> consumerNode)
        {
            var messageActivation = new MessageActivation<T>(consumerNode);
            return configurator.Add(messageActivation);
        }
    }
}