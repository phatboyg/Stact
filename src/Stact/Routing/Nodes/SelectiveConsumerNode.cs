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
namespace Stact.Routing.Nodes
{
    /// <summary>
    /// Selectively delivers a message to a consumer on the specified fiber.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SelectiveConsumerNode<T> :
        ProductionNode<T>,
        Activation<T>
    {
        readonly SelectiveConsumer<Message<T>> _selectiveConsumer;

        public SelectiveConsumerNode(RoutingEngine engine, SelectiveConsumer<Message<T>> selectiveConsumer,
                                     bool disableOnActivation = true)
            : base(engine, disableOnActivation)
        {
            _selectiveConsumer = selectiveConsumer;
        }

        public void Activate(RoutingContext<T> context)
        {
            Consumer<Message<T>> consumer = _selectiveConsumer(context.Message);
            if (consumer == null)
                return;

            Accept(context, message => consumer(message));
        }
    }
}