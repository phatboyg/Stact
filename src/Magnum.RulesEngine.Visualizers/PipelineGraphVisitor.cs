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
	using Pipeline;
	using Pipeline.Segments;
	using Pipeline.Visitors;

	public class PipelineGraphVisitor :
		AbstractPipeVisitor
	{
		private readonly Dictionary<Type, Color> _colors;
		private readonly List<Edge> _edges = new List<Edge>();
		private readonly Stack<Vertex> _stack = new Stack<Vertex>();
		private readonly Dictionary<int, Vertex> _vertices = new Dictionary<int, Vertex>();
		private Vertex _lastNodeVertex;

		public PipelineGraphVisitor()
		{
			_colors = new Dictionary<Type, Color>
				{
					{typeof (InputSegment), Color.Green},
					{typeof(FilterSegment), Color.Yellow},
					{typeof(RecipientListSegment), Color.Orange},
					{typeof(EndSegment), Color.Red},
					{typeof(MessageConsumerSegment), Color.Blue},
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


		public void Visit(Pipe pipe)
		{
			base.Visit(pipe);
		}

		protected override Pipe VisitInput(InputSegment input)
		{
			_lastNodeVertex = GetSink(input.GetHashCode(), () => "Input", typeof (InputSegment), input.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return GoDeep(() => base.VisitInput(input));
		}

		protected override Pipe VisitEnd(EndSegment end)
		{
			_lastNodeVertex = GetSink(end.GetHashCode(), () => "End", typeof (EndSegment), end.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return base.VisitEnd(end);
		}

		protected override Pipe VisitFilter(FilterSegment filter)
		{
			_lastNodeVertex = GetSink(filter.GetHashCode(), () => "Filter", typeof (FilterSegment), filter.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return GoDeep(() => base.VisitFilter(filter));
		}

		protected override Pipe VisitInterceptor(InterceptorSegment interceptor)
		{
			_lastNodeVertex = GetSink(interceptor.GetHashCode(), () => "Interceptor", typeof (InterceptorSegment), interceptor.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return GoDeep(() => base.VisitInterceptor(interceptor));
		}

		protected override Pipe VisitMessageConsumer(MessageConsumerSegment messageConsumer)
		{
			_lastNodeVertex = GetSink(messageConsumer.GetHashCode(), () => "Consumer", typeof (MessageConsumerSegment), messageConsumer.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return GoDeep(() => base.VisitMessageConsumer(messageConsumer));
		}

		protected override Pipe VisitIntervalMessageConsumer(IntervalMessageConsumerSegment messageConsumer)
		{
			_lastNodeVertex = GetSink(messageConsumer.GetHashCode(), () => "Consumer", typeof (IntervalMessageConsumerSegment), messageConsumer.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return GoDeep(() => base.VisitIntervalMessageConsumer(messageConsumer));
		}

		protected override Pipe VisitAsyncMessageConsumer(AsyncMessageConsumerSegment messageConsumer)
		{
			_lastNodeVertex = GetSink(messageConsumer.GetHashCode(), () => "Consumer", typeof (AsyncMessageConsumerSegment), messageConsumer.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return GoDeep(() => base.VisitAsyncMessageConsumer(messageConsumer));
		}

		protected override Pipe VisitRecipientList(RecipientListSegment recipientList)
		{
			_lastNodeVertex = GetSink(recipientList.GetHashCode(), () => "List", typeof (RecipientListSegment), recipientList.MessageType);

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex));

			return GoDeep(() => base.VisitRecipientList(recipientList));
		}

		private Pipe GoDeep(Func<Pipe> action)
		{
			_stack.Push(_lastNodeVertex);

			Pipe result = action();

			_stack.Pop();

			return result;
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