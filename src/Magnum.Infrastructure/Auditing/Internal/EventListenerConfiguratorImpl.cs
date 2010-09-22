// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Infrastructure.Auditing.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using Extensions;
	using Magnum.Channels;
	using NHibernate.Cfg;
	using NHibernate.Event;
	using Reflection;


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
			Guard.IsTrue(x => x.IsGenericTypeDefinition, eventType, "eventType", "Must be a generic type definition");
			Guard.AgainstNull(listenerFactory, "listenerFactory");
			Guard.AgainstNull(listenerAccessor, "listenerAccessor");

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