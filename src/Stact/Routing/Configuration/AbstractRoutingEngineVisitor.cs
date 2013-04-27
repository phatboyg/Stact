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
namespace Stact.Routing.Configuration
{
    using System.Linq;
    using Nodes;


    public abstract class AbstractRoutingEngineVisitor
    {
        protected virtual bool Visit(MessageRoutingEngine engine)
        {
            Visit(engine.Root);
            return true;
        }

        protected virtual bool Visit(RoutingEngine engine)
        {
            var routingEngine = engine as MessageRoutingEngine;
            Visit(routingEngine.Root);
            return true;
        }

        protected virtual bool Visit(Activation activation)
        {
            switch (activation.ActivationType)
            {
                case ActivationType.RootNode:
                    return Visit((RootNode)activation);

                default:
                    return true;
            }
        }

        protected virtual bool Visit<T>(Activation<T> activation)
        {
            return true;
        }

        protected virtual bool Visit(RootNode node)
        {
            return node.Activations.All(x => Visit((Activation)x));
        }

        protected virtual bool Visit<T>(AlphaNode<T> node)
        {
            return node.Successors.All(x => Visit(x));
        }

        protected virtual bool Visit<TInput, TOutput>(ConvertNode<TInput, TOutput> node)
            where TInput : TOutput
        {
            Visit(node.Output);
            return true;
        }

        protected virtual bool Visit<T>(ConsumerNode<T> node)
        {
            return true;
        }

        protected virtual bool Visit<T>(SelectiveConsumerNode<T> node)
        {
            return true;
        }

        protected virtual bool Visit<TActor>(StactActor<TActor> actor)
            where TActor : class
        {
            Visit(actor.Engine);
            return true;
        }
    }
}