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


	public class ConditionNode<TChannel> :
		Activation<TChannel>
	{
		readonly ActivationList<TChannel> _activations;
		readonly Filter<RoutingContext<TChannel>> _filter;
		readonly Expression<Filter<TChannel>> _filterExpression;

		public ConditionNode(Expression<Filter<TChannel>> filterExpression)
		{
			_activations = new ActivationList<TChannel>();

			_filterExpression = filterExpression;
			_filter = GetRoutingContextFilter(filterExpression).Compile();
		}

		public IEnumerable<Activation<TChannel>> Activations
		{
			get { return _activations; }
		}

		public Expression<Filter<TChannel>> FilterExpression
		{
			get { return _filterExpression; }
		}

		public void Activate(RoutingContext<TChannel> context)
		{
			if (!_filter(context))
				return;

			_activations.All(activation => context.Add(() => activation.Activate(context)));
		}

		static Expression<Filter<RoutingContext<TChannel>>> GetRoutingContextFilter(
			Expression<Filter<TChannel>> filterExpression)
		{
			ParameterExpression context = Expression.Parameter(typeof(RoutingContext<TChannel>), "value");

			PropertyInfo bodyProperty =
				ExtensionsToExpression.GetMemberPropertyInfo<RoutingContext<TChannel>, TChannel>(x => x.Body);

			MethodCallExpression body = Expression.Call(context, bodyProperty.GetGetMethod(true));

			InvocationExpression invokeFilter = Expression.Invoke(filterExpression, body);

			return Expression.Lambda<Filter<RoutingContext<TChannel>>>(invokeFilter, context);
		}
	}
}