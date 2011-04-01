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
namespace Stact.Routing
{
	using System;
	using Internal;


	public class RoutingContextProxy<T, TChannel> :
		RoutingContext<TChannel>
	{
		readonly TChannel _body;
		readonly DynamicRoutingContext<T> _context;

		public RoutingContextProxy(DynamicRoutingContext<T> context, TChannel body)
		{
			_context = context;
			_body = body;
		}

		public bool IsAlive
		{
			get { return _context.IsAlive; }
		}

		public void Add(Action action)
		{
			_context.Add(action);
		}

		public void Evict()
		{
			_context.Evict();
		}

		public TChannel Body
		{
			get { return _body; }
		}

		public RoutingContext<Stact.Routing.Internal.Tuple<TChannel, T2>> Join<T2>(RoutingContext<T2> other)
		{
			return DynamicRoutingContext.Create(_context.Engine, Body, other.Body);
		}

		public void IsAssignableTo<TChannel1>(Action<RoutingContext<TChannel1>> callback)
		{
			_context.IsAssignableTo<TChannel1>(callback);
		}
	}
}
