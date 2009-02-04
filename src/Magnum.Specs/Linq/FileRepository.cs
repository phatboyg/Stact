namespace Magnum.Common.Specs.Linq
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	using Magnum.Common.Reflection;

	public class FileRepository<T> : 
		IOrderedQueryable<T>,
		IQueryProvider,
		IDisposable
	{
		public IEnumerator<T> GetEnumerator()
		{
			return (Execute<IEnumerable<T>>(Expression)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (Execute<IEnumerable>(Expression)).GetEnumerator();
		}

		public Expression Expression
		{
			get
			{
				return Expression.Constant(this);
			}
		}

		public Type ElementType
		{
			get { return typeof (T); }
		}

		public IQueryProvider Provider
		{
			get { return this; }
		}

		public void Dispose()
		{

			
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return (this as IQueryProvider).CreateQuery<T>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			if (typeof(T) != typeof(TElement))
			{
			//	projectedQuery = new ProjectedQuery<T, S>(expression, this);

			//	return (IQueryable<S>)projectedQuery;

				throw new NotSupportedException();
			}

			MethodCallExpression methodCallExpression = MethodCallFinder.Find("Where", expression);
			if(methodCallExpression != null)
			{
				return (IQueryable<TElement>)((ConstantExpression)methodCallExpression.Arguments[0]).Value;
			}

			throw new NotSupportedException();
		}

		public object Execute(Expression expression)
		{

			throw new NotImplementedException();
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult)(this as IQueryProvider).Execute(expression);
		}
	}


	public class MethodCallFinder : 
		ExpressionVisitor
	{
		private readonly string _name;
		private MethodCallExpression _found;

		protected MethodCallFinder(string name)
		{
			_name = name;
		}

		private MethodCallExpression Found
		{
			get { return _found; }
		}


		public static MethodCallExpression Find(string name, Expression expression)
		{
			MethodCallFinder finder = new MethodCallFinder(name);

			finder.Visit(expression);

			return finder.Found;
		}

		protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == _name)
                _found = expression;

            Visit(expression.Arguments[0]);

            return expression;
		}
	}
}