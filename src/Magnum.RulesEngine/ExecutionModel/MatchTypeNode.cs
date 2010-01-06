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
namespace Magnum.RulesEngine.ExecutionModel
{
	using System;
	using Collections;

	public class MatchTypeNode :
		Node,
		Activation,
		ModelVisitorSite
	{
		private readonly MultiDictionary<Type, Activation> _types;

		public MatchTypeNode()
		{
			_types = new MultiDictionary<Type, Activation>(false);
		}

		public void Activate<T>(RuleContext<T> context)
		{
			_types[typeof (T)].Each(x => x.Activate(context));
		}

		public void Add<T>(Activation<T> successor)
		{
			_types.Add(typeof (T), new ActivationTranslater<T>(successor));
		}

		private class ActivationTranslater<T> :
			Activation,
			ModelVisitorSite
		{
			private readonly Activation<T> _activation;

			public ActivationTranslater(Activation<T> activation)
			{
				if (activation == null)
					throw new ArgumentNullException("activation", "Activation cannot be null");

				_activation = activation;
			}

			public void Activate<TInput>(RuleContext<TInput> context)
			{
				var ruleContext = (RuleContext<T>) context;

				_activation.Activate(ruleContext);
			}

			public bool Visit(ModelVisitor visitor)
			{
				var site = _activation as ModelVisitorSite;
				if(site != null)
					return site.Visit(visitor);

				return true;
			}
		}

		public bool Visit(ModelVisitor visitor)
		{
			return visitor.Visit(this, () => _types.Values.EachUntilFalse<ModelVisitorSite>(x => x.Visit(visitor)));
		}
	}
}