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
namespace Stact.Workflow.Internal
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Reflection;


	public class CurrentStateAccessor<TInstance> :
		StateAccessor<TInstance>
	{
		readonly State<TInstance> _defaultState;
		readonly FastProperty<TInstance, State> _property;

		public CurrentStateAccessor(Expression<Func<TInstance, State>> currentStateExpression, State<TInstance> defaultState)
		{
			_defaultState = defaultState;
			_property = currentStateExpression.WithPropertyExpression(x => new FastProperty<TInstance, State>(x, BindingFlags.NonPublic));
		}

		public State<TInstance> Get(TInstance instance)
		{
			return _property.Get(instance) as State<TInstance> ?? _defaultState;
		}

		public void Set(TInstance instance, State state)
		{
			_property.Set(instance, state);
		}
	}
}