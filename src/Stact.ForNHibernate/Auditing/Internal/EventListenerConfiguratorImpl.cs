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
namespace Stact.ForNHibernate.Auditing.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;
	using Stact;
	using NHibernate.Cfg;
	using NHibernate.Event;
	using Magnum.Reflection;


	public delegate TListener ListenerFactory<TListener>(UntypedChannel output, HashSet<Type> types);


	public class EventListenerConfiguratorImpl<TListener> :
		EventListenerConfigurator
	{
		readonly Expression<Func<EventListeners, TListener[]>> _listenerAccessor;
		readonly ListenerFactory<TListener> _listenerFactory;
		readonly HashSet<Type> _types;
		Type _eventType;

		public EventListenerConfiguratorImpl(Type eventType, ListenerFactory<TListener> listenerFactory,
		                                     Expression<Func<EventListeners, TListener[]>> listenerAccessor)
		{
			Magnum.Guard.IsTrue(x => x.IsGenericTypeDefinition, eventType, "eventType", "Must be a generic type definition");
			Magnum.Guard.AgainstNull(listenerFactory, "listenerFactory");
			Magnum.Guard.AgainstNull(listenerAccessor, "listenerAccessor");

			_types = new HashSet<Type>();
			_eventType = eventType;
			_listenerFactory = listenerFactory;
			_listenerAccessor = listenerAccessor;
		}

		public IEnumerable<Type> Types
		{
			get { return _types; }
		}

		public bool IsHandled<T>()
		{
			if (typeof(T).Implements(_eventType))
			{
				_types.Add(typeof(T).GetGenericTypeDeclarations(_eventType).First());
				return true;
			}

			return false;
		}

		public void ApplyTo(Configuration cfg, UntypedChannel channel)
		{
			if (_types.Count == 0)
				return;

			PropertyInfo propertyInfo = _listenerAccessor.GetMemberPropertyInfo();
			var property = new FastProperty<EventListeners, TListener[]>(propertyInfo);

			TListener listener = _listenerFactory(channel, _types);

			TListener[] existing = property.Get(cfg.EventListeners);
			if (existing == null || existing.Length == 0)
				property.Set(cfg.EventListeners, new[] {listener});
			else
				property.Set(cfg.EventListeners, existing.Concat(Enumerable.Repeat(listener, 1)).ToArray());
		}
	}
}