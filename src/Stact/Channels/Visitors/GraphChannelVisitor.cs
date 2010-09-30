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
namespace Stact.Channels.Visitors
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Extensions;
	using Graphing;


	public class GraphChannelVisitor :
		ChannelVisitor
	{
		readonly List<Edge> _edges = new List<Edge>();
		readonly Stack<Vertex> _stack = new Stack<Vertex>();
		readonly Dictionary<int, Vertex> _vertices = new Dictionary<int, Vertex>();
		Vertex _current;

		public ChannelGraphData GetGraphData()
		{
			return new ChannelGraphData(_vertices.Values, _edges);
		}

		Vertex GetVertex(int key, Func<string> getTitle, Type nodeType, Type objectType)
		{
			return _vertices.Retrieve(key, () =>
				{
					var newSink = new Vertex(nodeType, objectType, getTitle());

					return newSink;
				});
		}

		Channel<T> WithVertex<T>(Func<Channel<T>> scopedAction)
		{
			_stack.Push(_current);

			Channel<T> result = scopedAction();

			_stack.Pop();

			return result;
		}

		UntypedChannel WithVertex(Func<UntypedChannel> scopedAction)
		{
			_stack.Push(_current);

			UntypedChannel result = scopedAction();

			_stack.Pop();

			return result;
		}

		ChannelProvider<T> WithVertex<T>(Func<ChannelProvider<T>> scopedAction)
		{
			_stack.Push(_current);

			ChannelProvider<T> result = scopedAction();

			_stack.Pop();

			return result;
		}

		protected override Channel<T> Visitor<T>(ConsumerChannel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Consumer", typeof(ConsumerChannel<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(FilterChannel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Filter", typeof(FilterChannel<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(InstanceChannel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Instance", typeof(InstanceChannel<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(IntervalChannel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Interval", typeof(IntervalChannel<T>), typeof(ICollection<T>));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(InterceptorChannel<T> channel)
		{
			Trace.WriteLine("InterceptorChannel<{0}>".FormatWith(typeof(T).Name));

			return base.Visitor(channel);
		}

		protected override Channel<ICollection<T>> Visitor<T, TKey>(DistinctChannel<T, TKey> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Distinct", typeof(DistinctChannel<T, TKey>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<ICollection<T>> Visitor<T>(LastChannel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Last", typeof(LastChannel<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(AsyncResultChannel<T> channel)
		{
			Trace.WriteLine("AsyncResultChannel<{0}>, {1}".FormatWith(typeof(T).Name,
			                                                          channel.IsCompleted ? "Complete" : "Pending"));

			return base.Visitor(channel);
		}

		protected override UntypedChannel Visitor(ShuntChannel channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Shunt", typeof(ShuntChannel), typeof(object));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(ShuntChannel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Shunt", typeof(ShuntChannel<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(ChannelAdapter<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Adapter", typeof(ChannelAdapter<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(BroadcastChannel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Router", typeof(BroadcastChannel<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<TInput> Visitor<TInput, TOutput>(ConvertChannel<TInput, TOutput> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Convert", typeof(ConvertChannel<TInput,TOutput>), typeof(TInput));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override Channel<T> Visitor<T>(Channel<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Channel", typeof(Channel<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override UntypedChannel Visitor(UntypedChannel channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "UntypedChannel", typeof(UntypedChannel), typeof(object));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override UntypedChannel Visitor(ChannelAdapter channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Adapter", typeof(ChannelAdapter), typeof(object));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override UntypedChannel Visitor(BroadcastChannel channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Router", typeof(BroadcastChannel), typeof(object));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override UntypedChannel Visitor<T>(TypedChannelAdapter<T> channel)
		{
			_current = GetVertex(channel.GetHashCode(), () => "Cast", typeof(TypedChannelAdapter<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_stack.Peek(), _current, _current.TargetType.Name));

			return WithVertex(() => base.Visitor(channel));
		}

		protected override ChannelProvider<T> Visitor<T>(ChannelProvider<T> provider)
		{
			Trace.WriteLine("ChannelProvider<{0}>".FormatWith(typeof(T).Name));

			return base.Visitor(provider);
		}

		protected override ChannelProvider<T> Visitor<T>(DelegateChannelProvider<T> provider)
		{
			_current = GetVertex(provider.GetHashCode(), () => "Provider", typeof(DelegateChannelProvider<T>), typeof(T));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_current, _stack.Peek(), _current.TargetType.Name));

			return WithVertex(() => base.Visitor(provider));
		}

		protected override ChannelProvider<TChannel> Visitor<TConsumer, TChannel>(
			InstanceChannelProvider<TConsumer, TChannel> provider)
		{
			_current = GetVertex(provider.GetHashCode(), () => "Provider", typeof(InstanceChannelProvider<TConsumer, TChannel>),
			                     typeof(TConsumer));

			if (_stack.Count > 0)
				_edges.Add(new Edge(_current, _stack.Peek(), _current.TargetType.Name));

			return WithVertex(() => base.Visitor(provider));
		}

		protected override ChannelProvider<T> Visitor<T, TKey>(KeyedChannelProvider<T, TKey> provider)
		{
			Trace.WriteLine("KeyedChannelProvider<{0}>, Key = {1}".FormatWith(typeof(T).Name, typeof(TKey).Name));

			return base.Visitor(provider);
		}

		protected override ChannelProvider<T> Visitor<T>(ThreadStaticChannelProvider<T> provider)
		{
			Trace.WriteLine("ThreadStaticChannelProvider<{0}>".FormatWith(typeof(T).Name));

			return base.Visitor(provider);
		}

		protected override InterceptorFactory<T> Visitor<T>(InterceptorFactory<T> factory)
		{
			Trace.WriteLine("InterceptorFactory<{0}>".FormatWith(typeof(T).Name));

			return base.Visitor(factory);
		}
	}


//
//	public class GraphChannkkelVisitor :
//		ChannelVisitor
//		{
//
//			public new void Visit(Pipe pipe)
//			{
//				base.Visit(pipe);
//			}
//
//			protected override Pipe VisitInput(InputSegment input)
//			{
//				_lastNodeVertex = GetSink(input.GetHashCode(), () => "Input", typeof(InputSegment), input.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return Recurse(() => base.VisitInput(input));
//			}
//
//			protected override Pipe VisitEnd(EndSegment end)
//			{
//				_lastNodeVertex = GetSink(end.GetHashCode(), () => "End", typeof(EndSegment), end.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return base.VisitEnd(end);
//			}
//
//			protected override Pipe VisitFilter(FilterSegment filter)
//			{
//				_lastNodeVertex = GetSink(filter.GetHashCode(), () => "Filter", typeof(FilterSegment), filter.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return Recurse(() => base.VisitFilter(filter));
//			}
//
//			protected override Pipe VisitInterceptor(InterceptorSegment interceptor)
//			{
//				_lastNodeVertex = GetSink(interceptor.GetHashCode(), () => "Interceptor", typeof(InterceptorSegment), interceptor.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return Recurse(() => base.VisitInterceptor(interceptor));
//			}
//
//			protected override Pipe VisitMessageConsumer(MessageConsumerSegment messageConsumer)
//			{
//				_lastNodeVertex = GetSink(messageConsumer.GetHashCode(), () => "Consumer", typeof(MessageConsumerSegment), messageConsumer.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return Recurse(() => base.VisitMessageConsumer(messageConsumer));
//			}
//
//			protected override Pipe VisitIntervalMessageConsumer(IntervalMessageConsumerSegment messageConsumer)
//			{
//				_lastNodeVertex = GetSink(messageConsumer.GetHashCode(), () => "Consumer", typeof(IntervalMessageConsumerSegment), messageConsumer.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return Recurse(() => base.VisitIntervalMessageConsumer(messageConsumer));
//			}
//
//			protected override Pipe VisitAsyncMessageConsumer(AsyncMessageConsumerSegment messageConsumer)
//			{
//				_lastNodeVertex = GetSink(messageConsumer.GetHashCode(), () => "Consumer", typeof(AsyncMessageConsumerSegment), messageConsumer.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return Recurse(() => base.VisitAsyncMessageConsumer(messageConsumer));
//			}
//
//			protected override Pipe VisitRecipientList(RecipientListSegment recipientList)
//			{
//				_lastNodeVertex = GetSink(recipientList.GetHashCode(), () => "List", typeof(RecipientListSegment), recipientList.MessageType);
//
//				if (_stack.Count > 0)
//					_edges.Add(new Edge(_stack.Peek(), _lastNodeVertex, _lastNodeVertex.TargetType.Name));
//
//				return Recurse(() => base.VisitRecipientList(recipientList));
//			}
//
//			private Pipe Recurse(Func<Pipe> action)
//			{
//				_stack.Push(_lastNodeVertex);
//
//				Pipe result = action();
//
//				_stack.Pop();
//
//				return result;
//			}
//
//			private Vertex GetSink(int key, Func<string> getTitle, Type nodeType, Type objectType)
//			{
//				return _vertices.Retrieve(key, () =>
//				{
//					var newSink = new Vertex(nodeType, objectType, getTitle());
//
//					return newSink;
//				});
//			}
//		}

//	}
}