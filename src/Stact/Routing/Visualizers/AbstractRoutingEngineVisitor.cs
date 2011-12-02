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
namespace Stact.Routing.Visualizers
{
    using Magnum.Extensions;
    using Magnum.Reflection;
    using Nodes;


    public abstract class AbstractRoutingEngineVisitor<TVisitor> :
        ReflectiveVisitorBase<TVisitor>
        where TVisitor : AbstractRoutingEngineVisitor<TVisitor>
    {
        protected virtual bool Visit(DynamicRoutingEngine engine)
        {
            Visit(engine.Root);
            return true;
        }

        protected virtual bool Visit(RootNode node)
        {
            node.Activations.Each(typeChannel => Visit(typeChannel));
            return true;
        }

        protected virtual bool Visit<T>(AlphaNode<T> node)
        {
            IncreaseDepth();
            node.Successors.Each(activation => Visit(activation));
            DecreaseDepth();
            return true;
        }

        protected virtual bool Visit<TInput, TOutput>(ConvertNode<TInput, TOutput> node)
            where TInput : TOutput
        {
            IncreaseDepth();
            Visit(node.Output);
            DecreaseDepth();
            return true;
        }

        protected virtual bool Visit<T>(JoinNode<T> node)
        {
            IncreaseDepth();
            if (node.RightActivation as ConstantNode<T> != null)
                Visit(node.RightActivation);
            node.Activations.Each(activation => Visit(activation));
            DecreaseDepth();
            return true;
        }

        protected virtual bool Visit<T>(ConstantNode<T> node)
        {
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

        protected virtual bool Visit<T>(MessageNode<T> node)
        {
            IncreaseDepth();
            Visit(node.Output);
            DecreaseDepth();
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