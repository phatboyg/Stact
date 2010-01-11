// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.RulesEngine.Visualizers
{
	using System;
	using System.Collections.Generic;
	using System.Drawing;
	using CollectionExtensions;
	using ExecutionModel;

	public class RulesEngineGraphVisitor :
		AbstractModelVisitor<RulesEngineGraphVisitor>
	{
		private readonly Dictionary<Type, Color> _colors;
		private readonly List<Edge> _edges = new List<Edge>();
		private readonly Stack<Vertex> _stack = new Stack<Vertex>();
		private readonly Dictionary<int, Vertex> _vertices = new Dictionary<int, Vertex>();
		private Vertex _lastNodeVertex;

		public RulesEngineGraphVisitor()
		{
			_colors = new Dictionary<Type, Color>
				{
					{typeof (AlphaNode<>), Color.Red},
					{typeof (TypeNode<>), Color.Orange},
					{typeof (JoinNode<>), Color.Green},
					{typeof (ConditionNode<>), Color.Blue},
					{typeof (ActionNode<>), Color.Teal},
					{typeof (ConstantNode<>), Color.Magenta},
				};
		}

		public IEnumerable<Vertex> Vertices
		{
			get { return _vertices.Values; }
		}

		public IEnumerable<Edge> Edges
		{
			get { return _edges; }
		}

		protected override void IncreaseDepth()
		{
			if (_lastNodeVertex != null)
				_stack.Push(_lastNodeVertex);
		}

		protected override void DecreaseDepth()
		{
			if (_stack.Count > 0)
				_stack.Pop();
		}

		protected bool Visit(TypeDispatchNode node)
		{
			_lastNodeVertex = GetSink(node.GetHashCode(), () => "Rules", typeof (TypeDispatchNode), typeof (object));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return true;
		}

		protected bool Visit<T>(TypeNode<T> node)
		{
			_lastNodeVertex = GetSink(node.GetHashCode(), () => typeof(T).Name, typeof(TypeNode<>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return true;
		}

		protected bool Visit<T>(ConditionNode<T> node)
		{
			_lastNodeVertex = GetSink(node.GetHashCode(), () => node.Body, typeof(ConditionNode<>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return true;
		}

		protected bool Visit<T>(AlphaNode<T> node)
		{
			_lastNodeVertex = GetSink(node.GetHashCode(), () => "\u03B1", typeof(AlphaNode<>), typeof(T));

			if (_stack.Count > 0 && _stack.Peek().NodeType != typeof (JoinNode<>))
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return true;
		}

		protected bool Visit<T>(JoinNode<T> node)
		{
			_lastNodeVertex = GetSink(node.GetHashCode(), () => "J", typeof(JoinNode<>), typeof(T));

			if (_vertices.ContainsKey(node.RightActivation.GetHashCode()))
			{
				Vertex sink = _vertices[node.RightActivation.GetHashCode()];

				_edges.Add(new Edge(sink, _lastNodeVertex));
			}

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return true;
		}

		protected bool Visit<T>(ConstantNode<T> node)
		{
			_lastNodeVertex = GetSink(node.GetHashCode(), () => "C", typeof(ConstantNode<>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_lastNodeVertex, _stack.Peek()));

			return true;
		}

		protected bool Visit<T>(ActionNode<T> node)
		{
			_lastNodeVertex = GetSink(node.GetHashCode(), () => node.Body, typeof (ActionNode<>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return true;
		}

		private Vertex GetSink(int key, Func<string> getTitle, Type nodeType, Type objectType)
		{
			return _vertices.Retrieve(key, () =>
				{
					Color color = _colors.Retrieve(nodeType, () => Color.Black);

					var newSink = new Vertex(nodeType, objectType, getTitle(), color);

					return newSink;
				});
		}
	}
}