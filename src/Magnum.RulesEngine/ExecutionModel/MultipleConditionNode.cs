namespace Magnum.RulesEngine.ExecutionModel
{
	using System;
	using System.Linq.Expressions;

	public class MultipleConditionNode<TInput, TRef1>
		where TInput : class
		where TRef1 : class
	{
		private readonly Expression<Func<TInput, TRef1, bool>> _expression;

		public MultipleConditionNode(Expression<Func<TInput, TRef1, bool>> expression)
		{
			_expression = expression;
		}
	}
}