namespace Magnum.Core
{
	using System;
	using CheckHelpers;

	public static class Check
	{
		public static void That(object obj, ICheckExpression expression)
		{
			if (!expression.Validate(obj))
				throw new CheckException("Expectation Not Met: " + expression);
		}

		public static void That<TException>(object obj, ICheckExpression expression) where TException : Exception
		{
			if (!expression.Validate(obj))
				throw (TException)Activator.CreateInstance(typeof(TException), "Expectation Not Met: " + expression);
		}

		public static void Argument(string paramName, object obj, ICheckExpression expression)
		{
			if (!expression.Validate(obj))
			{
				if (expression is NullCheckExpression)
					throw new ArgumentNullException(paramName, expression.ToString());
				else
					throw new ArgumentException(expression.ToString(), paramName);
			}
		}
	}
}