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
	using Magnum.Collections;


	public class DynamicRoutingContext<T> :
		RoutingContext<T>
	{
		readonly DynamicRoutingEngine _engine;
		readonly T _message;

		public DynamicRoutingContext(DynamicRoutingEngine engine, T message)
		{
			_engine = engine;
			_message = message;
		}

		public void Add(Action action)
		{
			_engine.Add(action);
		}

		public T Body
		{
			get { return _message; }
		}

		public RoutingContext<Tuple<T, T2>> Join<T2>(RoutingContext<T2> second)
		{
			return new DynamicRoutingContext<Tuple<T, T2>>(_engine, new Tuple<T, T2>(Body, second.Body));
		}
	}
}