namespace Magnum.RulesEngine.Visualizers
{
	using System;
	using System.Text;
	using Reflection;
	using StateMachine;

//	public class StateMachineGraphVisitor :
//	ReflectiveVisitorBase<StateMachineGraphVisitor>,
//	IStateMachineInspector
//	{
//		private State CurrentState { get; set; }
//
//		public StateMachineGraphVisitor()
//		{
//		}
//
//		public void Inspect(object obj)
//		{
//			base.Visit(obj);
//		}
//
//		public void Inspect(object obj, Action action)
//		{
//			base.Visit(obj, () =>
//			{
//				action();
//				return true;
//			});
//		}
//
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
//		public bool Visit<T>(State<T> state)
//			where T : StateMachine<T>
//		{
//			Append(string.Format("During {0}{1}", state.Name, state == CurrentState ? " (Current)" : ""));
//
//			return true;
//		}
//
//		public bool Visit<T>(BasicEvent<T> state)
//			where T : StateMachine<T>
//		{
//			Append(string.Format("When {0} Occurs", state.Name));
//
//			return true;
//		}
//
//		public bool Visit<T>(TransitionToAction<T> action)
//			where T : StateMachine<T>
//		{
//			Append("Transition To " + action.NewState.Name);
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
//		private void AppendEventAction<T>(EventAction<T> eventAction)
//			where T : StateMachine<T>
//		{
//			Append(string.Format("Then"));
//		}
//
//		public bool Visit<T, TData>(DataEvent<T, TData> state)
//			where T : StateMachine<T>
//		{
//			Append(string.Format("When {0} Occurs Containing {1}", state.Name, typeof(TData).Name));
//
//			return true;
//		}
//	}

}