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
	using Magnum.Extensions;
	using Stact.Internal;


	public static class DynamicRoutingContext
	{
		public static RoutingContext<T1> Create<T1>(DynamicRoutingEngine engine, T1 item1)
		{
			return new DynamicRoutingContext<T1>(engine, item1);
		}

		public static RoutingContext<Tuple<T1, T2>> Create<T1, T2>(DynamicRoutingEngine engine, T1 item1, T2 item2)
		{
			return new DynamicRoutingContext<Tuple<T1, T2>>(engine, new Tuple<T1, T2>(item1, item2));
		}
	}


	public class DynamicRoutingContext<T> :
		RoutingContext<T>
	{
		readonly DynamicRoutingEngine _engine;
		readonly T _message;
		bool _isAlive = true;

		internal DynamicRoutingContext(DynamicRoutingEngine engine, T message)
		{
			_engine = engine;
			_message = message;
		}

		public DynamicRoutingEngine Engine
		{
			get { return _engine; }
		}

		public void Add(Action action)
		{
			_engine.Add(action);
		}

		public bool IsAlive
		{
			get { return _isAlive; }
		}

		public void Evict()
		{
			_isAlive = false;
		}

		public T Body
		{
			get
			{
				if (!_isAlive)
					throw new InvalidOperationException("The item has been evicted, type = " + typeof(T).ToShortTypeName());

				return _message;
			}
		}

		public void CanConvertTo<TChannel>(Action<RoutingContext<TChannel>> callback)
		{
			if (typeof(TChannel) == typeof(T))
				callback((RoutingContext<TChannel>)this);
			else
			{
				HeaderTypeAdapter<TChannel>.TryConvert(_message, x =>
				{
					var proxy = new RoutingContextProxy<T, TChannel>(this, x);

					callback(proxy);
				});
			}
		}

		public RoutingContext<Tuple<T, T2>> Join<T2>(RoutingContext<T2> other)
		{
			return DynamicRoutingContext.Create(_engine, Body, other.Body);
		}


		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != typeof(DynamicRoutingContext<T>))
				return false;

			return false;
		}

		public override int GetHashCode()
		{
			return _message.GetHashCode();
		}
	}
}