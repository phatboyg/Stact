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
namespace Magnum.RulesEngine
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using Collections;
	using ExecutionModel;

	public class SessionRuleContext<T> :
		RuleContext<T>
	{
		private readonly MultiDictionary<long, Node> _activations;
		private readonly Agenda _agenda = new PriorityQueueAgenda();
		private readonly StatefulSession _session;

		public SessionRuleContext(StatefulSession session, WorkingMemoryElement<T> item)
		{
			_session = session;
			_activations = new MultiDictionary<long, Node>(false);

			Element = item;
		}

		public WorkingMemoryElement<T> Element { get; private set; }

		public void AddElementToAlphaMemory(int key, WorkingMemoryElement<T> element, IEnumerable<Node> successors)
		{
			long superKey = key << 32 | element.GetHashCode();

			_activations.AddMany(superKey, successors);
		}

		public Type ItemType
		{
			get { return typeof (T); }
		}

		public void EnqueueAgendaAction(Action action)
		{
			_agenda.Add(action);
		}

		public void EnqueueAgendaAction(int priority, Action action)
		{
			_agenda.Add(priority, action);
		}

		public void DumpMemory()
		{
			_activations.Each(element =>
				{
					Trace.WriteLine("Element: " + element.Key);

					element.Value.Each(successor => { Trace.WriteLine("  Successor: " + successor.NodeType); });
				});
		}

		private class KeyComparer :
			IEqualityComparer<WorkingMemoryElement<T>>
		{
			public bool Equals(WorkingMemoryElement<T> x, WorkingMemoryElement<T> y)
			{
				if ((x == null || y == null) && x != y)
					return false;

				if (x == null && y == null)
					return true;

				return ReferenceEquals(x.Object, y.Object);
			}

			public int GetHashCode(WorkingMemoryElement<T> obj)
			{
				return obj == null ? 0 : obj.Object.GetHashCode();
			}
		}

		private class NodeComparer :
			IEqualityComparer<Node>
		{
			public bool Equals(Node x, Node y)
			{
				if ((x == null || y == null) && x != y)
					return false;

				if (x == null && y == null)
					return true;

				return ReferenceEquals(x, y);
			}

			public int GetHashCode(Node node)
			{
				return node == null ? 0 : node.GetHashCode();
			}
		}
	}
}