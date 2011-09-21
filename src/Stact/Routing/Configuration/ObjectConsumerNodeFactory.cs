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
namespace Stact.Routing.Configuration
{
    using Nodes;


    public class ObjectConsumerNodeFactory<T> :
        ConsumerNodeFactory
    {
        public RemoveActivation Create<TMessage>(Consumer<TMessage> consumer, RoutingEngineConfigurator configurator)
        {
            var requestConsumer = consumer as Consumer<T>;
            var consumerNode = new ConsumerNode<T>(configurator.Engine, requestConsumer);

            return configurator.Add(consumerNode);
        }

        public RemoveActivation Create<T1>(SelectiveConsumer<T1> consumer, RoutingEngineConfigurator configurator)
        {
            var requestConsumer = consumer as SelectiveConsumer<T>;
            var consumerNode = new SelectiveConsumerNode<T>(configurator.Engine, requestConsumer);

            return configurator.Add(consumerNode);
        }
    }
}