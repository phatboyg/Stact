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
	using Stact.Internal;


	public class AlphaNode<TChannel> :
		Activation,
		Activation<TChannel>
	{
		readonly ActivationList<TChannel> _activations;
		readonly MessageList<TChannel> _messages;

		public AlphaNode()
		{
			_messages = new MessageList<TChannel>();
			_activations = new ActivationList<TChannel>();
		}

		public IEnumerable<Activation<TChannel>> Activations
		{
			get { return _activations; }
		}

		public void Activate<T>(RoutingContext<T> message)
		{
			HeaderTypeAdapter<RoutingContext<TChannel>>.TryConvert(message, Activate);
		}

		public void Activate(RoutingContext<TChannel> context)
		{
			_messages.Add(context);

			_activations.All(activation => context.Add(() => activation.Activate(context)));
		}

		/// <summary>
		/// Adds an activation, activating the new activation for every message in the list
		/// that has already been activated on the AlphaNode.
		/// </summary>
		/// <param name="activation"></param>
		public void AddActivation(Activation<TChannel> activation)
		{
			_activations.Add(activation);

			_messages.All(context => context.Add(() => activation.Activate(context)));
		}
	}


	/*
	 * public class AlphaNode<T> :
		Node,
		Activation<T>,
		RightActivation<T>
	{
		private readonly SuccessorSet<T> _successors;

		public AlphaNode()
		{
			_successors = new SuccessorSet<T>();
		}

		public void Activate(RuleContext<T> context)
		{
			BetaMemory<T> betaMemory = context.GetBetaMemory(GetHashCode(), () => new BetaMemory<T>(_successors));

			betaMemory.Activate(context);
		}

		public bool Visit(NodeVisitor visitor)
		{
			return visitor.Visit(this, () => _successors.Visit(visitor));
		}

		public bool RightActivate(RuleContext<T> context)
		{
			RightActivation<T> betaMemory = context.GetBetaMemory(GetHashCode(), () => new BetaMemory<T>(_successors));

			return betaMemory.RightActivate(context);
		}

		public void AddSuccessor(params Activation<T>[] successors)
		{
			successors.Each(x => _successors.Add(x));
		}

		public JoinNode<T> GetConstantJoinNode()
		{
			return _successors
				.Get(x => x.RightActivation.GetType() == typeof (ConstantNode<T>), () => new JoinNode<T>(new ConstantNode<T>()));
		}

		public JoinNode<T> GetAlphaJoinNode(AlphaNode<T> node)
		{
			return _successors
				.Get(x => ReferenceEquals(x.RightActivation, node), () => new JoinNode<T>(node));
		}
	}*/
}