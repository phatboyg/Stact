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
		Activation
	{
		private readonly MultiDictionary<Type, Activation> _alphaNodes;

		public MatchTypeNode()
		{
			_alphaNodes = new MultiDictionary<Type, Activation>(false);
		}

		public NodeType NodeType
		{
			get { return NodeType.MatchType; }
		}

		public void Activate<T>(RuleContext<T> context)
		{
			_alphaNodes[typeof (T)].Each(x => x.Activate(context));
		}

		public void Add<T>(Activation<T> successor)
		{
			_alphaNodes.Add(typeof (T), new ActivationTranslater<T>(successor));
		}

		private class ActivationTranslater<T> :
			Activation
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
		}
	}
}