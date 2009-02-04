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
	using System.Linq.Expressions;
	using System.Text;
	using Reflection;

	public class StateMachineInspector :
		ReflectiveVisitorBase<StateMachineInspector>,
		IStateMachineInspector
	{
		private int _depth = 0;
		private StringBuilder _text = new StringBuilder();

		public StateMachineInspector()
			: base("Inspect")
		{
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

		public bool Inspect<T>(State<T> state) 
			where T : StateMachine<T>
		{
			Append(string.Format("During {0}", state.Name));

			return true;
		}

		public bool Inspect<T>(BasicEvent<T> state)
			where T : StateMachine<T>
		{
			Append(string.Format("When {0} Occurs", state.Name));

			return true;
		}

		public bool Inspect<T,V>(StateEventAction<T,V> eventAction)
			where T : StateMachine<T> 
			where V : class
		{
			if(eventAction.Condition != null)
			{
				StateMachineExpressionInspector expressionInspector = new StateMachineExpressionInspector();

				string result = expressionInspector.Inspect(eventAction.Condition);
				
				Append(string.Format("If {0}", result));
			}

			AppendEventAction(eventAction);

			return true;
		}

		public bool Inspect<T>(StateEventAction<T> eventAction)
			where T : StateMachine<T>
		{
			AppendEventAction(eventAction);

			return true;
		}

		private void AppendEventAction<T>(StateEventAction<T> eventAction) 
			where T : StateMachine<T>
		{
			if(eventAction.EventAction != null)
				Append(string.Format("(custom action)"));

			if(eventAction.ResultState != null)
				Append(string.Format("Transition To {0}", eventAction.ResultState.Name));
		}

		public bool Inspect<T,V>(DataEvent<T,V> state)
			where T : StateMachine<T>
		{
			Append(string.Format("When {0} Occurs Containing {1}", state.Name, typeof(V).Name));

			return true;
		}

		protected override void DecreaseDepth()
		{
			_depth--;
		}

		protected override void IncreaseDepth()
		{
			_depth++;
		}

		private void Pad()
		{
			_text.Append(new string('\t', _depth));
		}

		private void Append(string text)
		{
			Pad();

			_text.AppendFormat(text).AppendLine();
		}

		public String Text
		{
			get { return _text.ToString(); }
		}

		public static void Trace(IStateMachineInspectorSite machine)
		{
			StateMachineInspector inspector = new StateMachineInspector();

			machine.Inspect(inspector);

			System.Diagnostics.Trace.WriteLine(inspector.Text);
		}
	}

	public class StateMachineExpressionInspector :
		ExpressionVisitor
	{
		private readonly StringBuilder _text = new StringBuilder();

		public string Inspect(Expression expression)
		{
			base.Visit(expression);

			return _text.ToString();
		}

		protected override Expression VisitMemberAccess(MemberExpression m)
		{
			_text.Append(string.Format("{0}", m.Member.Name));

			return base.VisitMemberAccess(m);
		}

		protected override Expression Visit(Expression exp)
		{
			switch(exp.NodeType)
			{
				case ExpressionType.MemberAccess:
				case ExpressionType.Parameter:
					break;

				default:
					_text.AppendFormat("({0})", exp.NodeType);
					break;
			}

			return base.Visit(exp);
		}
		
	}
}