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


	/// <summary>
	/// The basic functionality of a production node that deals with
	/// evicting the message from the routing engine and dispatching it
	/// to the specified delegate
	/// </summary>
	/// <typeparam name="TChannel">The message type</typeparam>
	public class ProductionNode<TChannel>
	{
		readonly bool _disableOnActivation;
		bool _enabled;

		protected ProductionNode(bool disableOnActivation)
		{
			_disableOnActivation = disableOnActivation;
			_enabled = true;
		}

		public bool Enabled
		{
			get { return _enabled; }
		}

		protected void Accept(RoutingContext<TChannel> context, Action<TChannel> callback)
		{
			TChannel body = context.Body;
			context.Evict();

			context.Add(() =>
			{
				if (_disableOnActivation)
					_enabled = false;

				callback(body);
			});
		}
	}
}