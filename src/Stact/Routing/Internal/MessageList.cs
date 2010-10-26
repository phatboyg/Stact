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
	using Magnum.Extensions;


	/// <summary>
	/// An AlphaMemory retains a list of messages and automatically removes
	/// messages from the list when they are evicted from memory based on the
	/// context property IsAvailable
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	public class MessageList<TChannel>
	{
		readonly IList<RoutingContext<TChannel>> _messages;

		public MessageList()
		{
			_messages = new List<RoutingContext<TChannel>>();
		}

		public void Add(RoutingContext<TChannel> message)
		{
			_messages.Add(message);
		}

		public void All(Action<RoutingContext<TChannel>> callback)
		{
			_messages.Each(callback);
		}

		public void Any(RoutingContext<TChannel> match, Action<RoutingContext<TChannel>> callback)
		{
			if (_messages.Contains(match))
				callback(match);
		}
	}
}