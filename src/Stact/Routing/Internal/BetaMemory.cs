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
	/// A beta channel supports activation via a typed input and activates
	/// any successors upon receipt
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BetaMemory<T> :
		Activation<T>,
		RightActivation<T>
	{
		readonly ActivationList<T> _activations;
		readonly MessageList<T> _messages;

		public BetaMemory()
		{
			_messages = new MessageList<T>();
			_activations = new ActivationList<T>();
		}

		public IEnumerable<Activation<T>> Activations
		{
			get { return _activations; }
		}

		public void Activate(RoutingContext<T> context)
		{
			_messages.Add(context);

			_activations.All(activation => context.Add(() => activation.Activate(context)));
		}

		public void RightActivate(Action<RoutingContext<T>> callback)
		{
			_messages.All(callback);
		}

		public void RightActivate(RoutingContext<T> context, Action<RoutingContext<T>> callback)
		{
			_messages.Any(context, callback);
		}

		public void AddActivation(Activation<T> activation)
		{
			_activations.Add(activation);

			_messages.All(context => context.Add(() => activation.Activate(context)));
		}

		public void RemoveActivation(Activation<T> activation)
		{
			_activations.Remove(activation);
		}
	}
}