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


	/// <summary>
	/// A JoinNode with a single type argument joins the output of the left side network
	/// to the right-side network. A single type will only join to the same message that 
	/// was passed through the left side network to the alpha nodes
	/// 
	/// BetaMemory is used since it keeps track of all valid activations (joins)
	/// 
	/// The unjoined messages are discarded (do they need to be kept, will right activations
	/// ever change, requiring a reevaluation of the join?)
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class JoinNode<T> :
		Activation<T>,
		RightActivation<T>
	{
		readonly BetaMemory<T> _betaMemory;
		readonly RightActivation<T> _rightActivation;

		public JoinNode(RightActivation<T> rightActivation)
		{
			_betaMemory = new BetaMemory<T>();

			_rightActivation = rightActivation;
		}

		public RightActivation<T> RightActivation
		{
			get { return _rightActivation; }
		}

		public IEnumerable<Activation<T>> Activations
		{
			get { return _betaMemory.Successors; }
		}

		public void Activate(RoutingContext<T> context)
		{
			_rightActivation.RightActivate(context, match => _betaMemory.Activate(match));
		}

		public bool IsAlive
		{
			get { return true; }
		}

		public void RightActivate(Func<RoutingContext<T>, bool> callback)
		{
			_betaMemory.RightActivate(callback);
		}

		public void RightActivate(RoutingContext<T> context, Action<RoutingContext<T>> callback)
		{
			_betaMemory.RightActivate(context, callback);
		}

		public void AddActivation(Activation<T> activation)
		{
			_betaMemory.AddActivation(activation);
		}

		public void RemoveActivation(Activation<T> activation)
		{
			_betaMemory.RemoveActivation(activation);
		}
	}
}