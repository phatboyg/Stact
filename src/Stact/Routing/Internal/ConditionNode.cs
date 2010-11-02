// Copyright 2010 Chris Patterson
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
namespace Stact.Routing.Internal
{
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;


	public class ConditionNode<T> :
		Activation<T>
	{
		readonly ActivationList<T> _activations;
		readonly Filter<RoutingContext<T>> _filter;
		readonly Expression<Filter<T>> _filterExpression;

		public ConditionNode(Expression<Filter<T>> filterExpression)
		{
			_activations = new ActivationList<T>();

			_filterExpression = filterExpression;
			_filter = GetRoutingContextFilter(filterExpression).Compile();
		}

		public IEnumerable<Activation<T>> Activations
		{
			get { return _activations; }
		}

		public Expression<Filter<T>> FilterExpression
		{
			get { return _filterExpression; }
		}

		public void Activate(RoutingContext<T> context)
		{
			if (!_filter(context))
				return;

			_activations.All(activation => activation.Activate(context));
		}

		public bool IsAlive
		{
			get { return true; }
		}

		static Expression<Filter<RoutingContext<T>>> GetRoutingContextFilter(
			Expression<Filter<T>> filterExpression)
		{
			ParameterExpression context = Expression.Parameter(typeof(RoutingContext<T>), "value");

			PropertyInfo bodyProperty =
				ExtensionsToExpression.GetMemberPropertyInfo<RoutingContext<T>, T>(x => x.Body);

			MethodCallExpression body = Expression.Call(context, bodyProperty.GetGetMethod(true));

			InvocationExpression invokeFilter = Expression.Invoke(filterExpression, body);

			return Expression.Lambda<Filter<RoutingContext<T>>>(invokeFilter, context);
		}
	}
}