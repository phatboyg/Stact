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
	using System;
	using System.Collections.Generic;


	public class JoinNode<T1, T2> :
		Activation<T1>,
		RightActivation<Tuple<T1, T2>>
	{
		readonly BetaMemory<Tuple<T1, T2>> _betaMemory;
		readonly RightActivation<T2> _rightActivation;

		public JoinNode(RightActivation<T2> rightActivation)
		{
			_betaMemory = new BetaMemory<Tuple<T1, T2>>();

			_rightActivation = rightActivation;
		}

		public RightActivation<T2> RightActivation
		{
			get { return _rightActivation; }
		}

		public IEnumerable<Activation<Tuple<T1, T2>>> Activations
		{
			get { return _betaMemory.Activations; }
		}

		public void Activate(RoutingContext<T1> context)
		{
			_rightActivation.RightActivate(match =>
				{
					if (!context.IsAlive)
						return false;

					_betaMemory.Activate(context.Join(match));
					return true;
				});
		}

		public bool IsAlive
		{
			get { return true; }
		}

		public void RightActivate(Func<RoutingContext<Tuple<T1, T2>>, bool> callback)
		{
			_betaMemory.RightActivate(callback);
		}

		public void RightActivate(RoutingContext<Tuple<T1, T2>> context, Action<RoutingContext<Tuple<T1, T2>>> callback)
		{
			_betaMemory.RightActivate(context, callback);
		}

		public void AddActivation(Activation<Tuple<T1, T2>> activation)
		{
			_betaMemory.AddActivation(activation);
		}

		public void RemoveActivation(Activation<Tuple<T1, T2>> activation)
		{
			_betaMemory.RemoveActivation(activation);
		}
	}
}