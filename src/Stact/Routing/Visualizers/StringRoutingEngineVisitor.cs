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
    using System.Text;
    using Magnum.Extensions;
    using Nodes;


    public class StringRoutingEngineVisitor :
        AbstractRoutingEngineVisitor<StringRoutingEngineVisitor>
    {
        int _depth;
        string _padding = "";
        StringBuilder _sb = new StringBuilder();

        public StringRoutingEngineVisitor(RoutingEngine engine)
        {
            Visit(engine);
        }


        protected override void IncreaseDepth()
        {
            _depth += 2;
            _padding = new string(' ', _depth);

            base.IncreaseDepth();
        }

        protected override void DecreaseDepth()
        {
            _depth -= 2;
            _padding = new string(' ', _depth);

            base.DecreaseDepth();
        }

        void AppendLine(string line)
        {
            _sb.AppendFormat("{0}{1}", _padding, line).AppendLine();
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        protected override bool Visit(DynamicRoutingEngine engine)
        {
            AppendLine(engine.GetType().ToShortTypeName());

            return base.Visit(engine);
        }

        protected override bool Visit(RootNode router)
        {
            AppendLine(router.GetType().ToShortTypeName());

            return base.Visit(router);
        }

        protected override bool Visit<T>(AlphaNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName());

            return base.Visit(node);
        }

        protected override bool Visit<TInput, TOutput>(ConvertNode<TInput, TOutput> node)
        {
            AppendLine(node.GetType().ToShortTypeName());

            return base.Visit(node);
        }

        protected override bool Visit<T>(JoinNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName() + ", Count: " + node.Count);

            return base.Visit(node);
        }

        protected override bool Visit<T1, T2>(JoinNode<T1, T2> node)
        {
            AppendLine(node.GetType().ToShortTypeName());

            return base.Visit(node);
        }

        protected override bool Visit<T>(ConstantNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName());

            return base.Visit(node);
        }

        protected override bool Visit<T>(ConsumerNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName());

            return base.Visit(node);
        }

        protected override bool Visit<T>(SelectiveConsumerNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName());

            return base.Visit(node);
        }

        protected override bool Visit<T>(MessageNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName());
            return base.Visit<T>(node);
        }

        protected override bool Visit<T>(RequestNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName());
            return base.Visit<T>(node);
        }

        protected override bool Visit<T>(ResponseNode<T> node)
        {
            AppendLine(node.GetType().ToShortTypeName());
            return base.Visit<T>(node);
        }
    }
}