namespace Magnum.RulesEngine.Specs
{
	using System;
	using System.Linq.Expressions;

	public class Condition<T> : 
		ICondition 
		where T : class
	{
		private readonly Expression<Func<T, bool>> _expression;
		private readonly Func<T, bool> _function;

		public Condition(Expression<Func<T, bool>> expression)
		{
			_expression = expression;
			_function = _expression.Compile();
		}

		public bool Evaluate(object obj)
		{
			if (obj.GetType() != typeof(T))
				return false;

			return _function(obj as T);
		}
	}

	public class Condition<T1,T2> : 
		ICondition 
		where T1 : class
		where T2 : class
	{
		private readonly Expression<Func<T1, T2, bool>> _expression;
		private readonly Func<T1, T2, bool> _function;

		public Condition(Expression<Func<T1, T2, bool>> expression)
		{
			_expression = expression;
			_function = _expression.Compile();
		}

		public bool Evaluate(object obj)
		{
			throw new NotSupportedException();
		}
	}
}