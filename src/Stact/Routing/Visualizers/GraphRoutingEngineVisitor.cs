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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Collections;
    using Magnum.Extensions;
    using Magnum.Graphing;
    using Nodes;
    using Stact.Internal;


    public class GraphRoutingEngineVisitor :
        AbstractRoutingEngineVisitor<GraphRoutingEngineVisitor>
    {
        readonly List<Edge> _edges = new List<Edge>();
        readonly Agenda _operations = new Agenda();
        readonly Stack<Vertex> _stack = new Stack<Vertex>();
        readonly Cache<int, Vertex> _vertices = new Cache<int, Vertex>();
        Vertex _current;

        public GraphRoutingEngineVisitor(RoutingEngine engine)
        {
            Visit(engine);

            _operations.Run();
        }

        public RoutingEngineGraphData GetGraphData()
        {
            return new RoutingEngineGraphData(_vertices, _edges);
        }

        Vertex GetVertex(int key, Func<string> getTitle, Type nodeType, Type objectType)
        {
            return _vertices.Retrieve(key, _ =>
            {
                var newSink = new Vertex(nodeType, objectType, getTitle());

                return newSink;
            });
        }

        bool WithVertex(Func<bool> scopedAction)
        {
            _stack.Push(_current);

            bool result = scopedAction();

            _stack.Pop();

            return result;
        }

        void AddEdge(Edge edge)
        {
            if (_edges.Any(existing => edge.From == existing.From && edge.To == existing.To && edge.Title == existing.Title))
                return;

            _edges.Add(edge);
        }

        protected override bool Visit(RootNode node)
        {
            _current = GetVertex(node.GetHashCode(), () => "Root", typeof(RootNode), null);

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<T>(AlphaNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "\u03B1", typeof(AlphaNode<>), typeof(T));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<T>(JoinNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "J", typeof(JoinNode<>), typeof(T));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<T1, T2>(JoinNode<T1, T2> node)
        {
            Vertex self = _current = GetVertex(node.GetHashCode(), () => "J", typeof(JoinNode<,>), typeof(System.Tuple<T1, T2>));

            LinkFromParent();
            LinkRightActivation(node.RightActivation, self);

            return WithVertex(() => base.Visit(node));
        }

        void LinkRightActivation<T>(RightActivation<T> rightActivation, Vertex current)
        {
            _operations.Add(0, () =>
            {
                _vertices.WithValue(rightActivation.GetHashCode(), sink =>
                {
                    AddEdge(new Edge(sink, current, sink.TargetType.ToShortTypeName()));
                });
            });
        }

        protected override bool Visit<T>(ConstantNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "C", typeof(ConstantNode<>), typeof(T));

            LinkToParent();

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<TInput,TOutput>(ConvertNode<TInput,TOutput> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "\u21A7", typeof(ConvertNode<,>), typeof(TOutput));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<T>(SelectiveConsumerNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "P?", typeof(SelectiveConsumerNode<>), typeof(T));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<T>(ConsumerNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "P", typeof(ConsumerNode<>), typeof(T));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }
        
        protected override bool Visit<T>(MessageNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "P", typeof(MessageNode<>), typeof(T));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<T>(RequestNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "P", typeof(RequestNode<>), typeof(T));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }

        protected override bool Visit<T>(ResponseNode<T> node)
        {
            _current = GetVertex(node.GetHashCode(), () => "P", typeof(ResponseNode<>), typeof(T));

            LinkFromParent();

            return WithVertex(() => base.Visit(node));
        }

        void LinkFromParent()
        {
            if (_stack.Count > 0)
                AddEdge(new Edge(_stack.Peek(), _current, _current.TargetType.ToShortTypeName()));
        }

        void LinkToParent()
        {
            if (_stack.Count > 0)
                AddEdge(new Edge(_current, _stack.Peek(), _stack.Peek().TargetType.ToShortTypeName()));
        }
    }
}