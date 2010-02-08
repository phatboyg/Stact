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
namespace Magnum.StateMachine
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using CollectionExtensions;
	using Graphing;
	using Reflection;

	public class GraphStateMachineVisitor<TStateMachine> :
		ReflectiveVisitorBase<GraphStateMachineVisitor<TStateMachine>>,
		IStateMachineInspector
		where TStateMachine : StateMachine<TStateMachine>
	{
		private readonly List<Edge> _edges = new List<Edge>();
		private readonly Dictionary<string, Vertex> _eventVertices = new Dictionary<string, Vertex>();
		private readonly Dictionary<string, Vertex> _vertices = new Dictionary<string, Vertex>();
		private Vertex _currentEventVertex;
		private Vertex _currentStateVertex;

		public GraphStateMachineVisitor()
			: base("Inspect")
		{
		}

		public IEnumerable<Vertex> Vertices
		{
			get { return _vertices.Values.Union(_eventVertices.Values); }
		}

		public IEnumerable<Edge> Edges
		{
			get { return _edges; }
		}

		public void Inspect(object obj)
		{
			base.Visit(obj);
		}

		public void Inspect(object obj, Action action)
		{
			base.Visit(obj, () =>
				{
					action();
					return true;
				});
		}

		public bool Inspect<T>(T machine)
			where T : StateMachine<T>
		{
			return true;
		}

		public bool Inspect<T>(State<T> state)
			where T : StateMachine<T>
		{
			_currentStateVertex = GetStateVertex(state.Name, () => state.Name, typeof (State), typeof (T));

			return true;
		}

		public bool Inspect<T>(BasicEvent<T> eevent)
			where T : StateMachine<T>
		{
			_currentEventVertex = GetEventVertex(eevent.Name, () => eevent.Name, typeof (Event), typeof (void));

			_edges.Add(new Edge(_currentStateVertex, _currentEventVertex, eevent.Name));

			return true;
		}


		public bool Inspect<T, V>(DataEvent<T, V> eevent)
			where T : StateMachine<T>
		{
			_currentEventVertex = GetEventVertex(eevent.Name, () => eevent.Name, typeof (Event), typeof(V));

			_edges.Add(new Edge(_currentStateVertex, _currentEventVertex, eevent.Name));

			return true;
		}

		public bool Inspect<T>(TransitionToAction<T> action)
			where T : StateMachine<T>
		{
			Vertex targetStateVertex = GetStateVertex(action.NewState.Name, () => action.NewState.Name, typeof (State), typeof (T));

			_edges.Add(new Edge(_currentEventVertex, targetStateVertex, _currentEventVertex.Title));

			return true;
		}

		//		public bool Visit<T>(LambdaAction<T> action)
		//			where T : StateMachine<T>
		//		{
		//			Append("Action<" + typeof(T).Name + ">");
		//			return true;
		//		}
		//
		//		public bool Visit<T, TData>(LambdaAction<T, TData> action)
		//			where T : StateMachine<T>
		//			where TData : class
		//		{
		//			Append("Action<" + typeof(T).Name + "," + typeof(TData).Name + ">");
		//			return true;
		//		}
		//
		//		public bool Visit<T>(ExpressionAction<T> action)
		//			where T : StateMachine<T>
		//		{
		//			string result = new StateMachineExpressionInspector().Inspect(action.Expression);
		//
		//			Append(result);
		//			return true;
		//		}
		//
		//		public bool Visit<T, TData>(ExpressionAction<T, TData> action)
		//			where T : StateMachine<T>
		//			where TData : class
		//		{
		//			string result = new StateMachineExpressionInspector().Inspect(action.Expression);
		//
		//			Append(result);
		//			return true;
		//		}
		//
		//		public bool Visit<T, TData>(DataEventAction<T, TData> eventAction)
		//			where T : StateMachine<T>
		//			where TData : class
		//		{
		//			if (eventAction.Condition != null)
		//			{
		//				string result = new StateMachineExpressionInspector().Inspect(eventAction.Condition);
		//
		//				Append(string.Format("If {0}", result));
		//			}
		//
		//			AppendEventAction(eventAction);
		//
		//			return true;
		//		}
		//
		//		public bool Visit<T>(BasicEventAction<T> eventAction)
		//			where T : StateMachine<T>
		//		{
		//			AppendEventAction(eventAction);
		//
		//			return true;
		//		}
		//

		private Vertex GetStateVertex(string name, Func<string> getTitle, Type nodeType, Type objectType)
		{
			return _vertices.Retrieve(name, () =>
				{
					var newSink = new Vertex(nodeType, objectType, getTitle());

					return newSink;
				});
		}

		private Vertex GetEventVertex(string name, Func<string> getTitle, Type nodeType, Type objectType)
		{
			return _eventVertices.Retrieve(name, () =>
				{
					var newSink = new Vertex(nodeType, objectType, getTitle());

					return newSink;
				});
		}
	}
}