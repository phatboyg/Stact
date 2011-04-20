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


	public class BodyNode<T> :
		Activation<Message<T>>
	{
		readonly BetaMemory<T> _betaMemory;

		public BodyNode()
		{
			_betaMemory = new BetaMemory<T>();
		}

		public BetaMemory<T> BetaMemory
		{
			get { return _betaMemory; }
		}

		public bool Enabled
		{
			get { return true; }
		}

		public IEnumerable<Activation<T>> Activations
		{
			get { return _betaMemory.Successors; }
		}

		public void Activate(RoutingContext<Message<T>> context)
		{
			context.CanConvertTo<T>(x => _betaMemory.Activate(x));
		}
	}


	public class BodyNode<T1In, T2In, T1, T2> :
		Activation<Tuple<T1In, T2In>>
	{
		readonly BetaMemory<Tuple<T1, T2>> _betaMemory;

		public BodyNode()
		{
			_betaMemory = new BetaMemory<Tuple<T1, T2>>();
		}

		public BetaMemory<Tuple<T1, T2>> BetaMemory
		{
			get { return _betaMemory; }
		}

		public IEnumerable<Activation<Tuple<T1, T2>>> Activations
		{
			get { return _betaMemory.Successors; }
		}

		public bool Enabled
		{
			get { return true; }
		}

		public void Activate(RoutingContext<Tuple<T1In, T2In>> context)
		{
			context.CanConvertTo<Tuple<T1, T2>>(x => _betaMemory.Activate(x));
		}
	}
}