// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.StateMachine
{
	using System;
	using System.Collections.Generic;
	using Magnum.Channels;
	using Magnum.Collections;
	using Magnum.Extensions;
	using Magnum.StateMachine.ChannelConfiguration;


	public class StateMachineBinding<T, TKey>
		where T : StateMachine<T>
	{
		readonly Cache<Event, EventBinder<T, TKey>> _binders = new Cache<Event, EventBinder<T, TKey>>();

		readonly Cache<Event, StateMachineEventInspectorResult<T>> _events =
			new Cache<Event, StateMachineEventInspectorResult<T>>();

		KeyAccessor<T, TKey> _identityAccessor;

		protected void Id(KeyAccessor<T, TKey> keyAccessor)
		{
			_identityAccessor = keyAccessor;
		}

		protected EventBinder<T, TKey, TEvent> Bind<TEvent>(Event<TEvent> @event, Func<TEvent, TKey> keyAccessor)
		{
			Func<Event, EventBinder<T, TKey>> onMissing = key => new DataEventBinder<T, TKey, TEvent>(keyAccessor);

			return (EventBinder<T, TKey, TEvent>)_binders.Retrieve(@event, onMissing);
		}

		public void ForEachEvent(Action<Event, EventBinder<T, TKey>, StateMachineEventInspectorResult<T>> callback)
		{
			_binders.Each((@event, binder) =>
				{
					var result = _events[@event];

					callback(@event, binder, result);
				});
		}

		public void Validate(T instance)
		{
			Guard.AgainstNull(instance);

			if (_identityAccessor == null)
				throw new InvalidOperationException("No identify expression defined for " + typeof(T).ToShortTypeName());

			var inspector = new StateMachineEventInspector<T>();
			instance.Inspect(inspector);

			IEnumerable<StateMachineEventInspectorResult<T>> results = inspector.GetResults();
			foreach (var result in results)
			{
				if (!_binders.Has(result.GenericEvent))
				{
					throw new InvalidOperationException("NO binding found for event type: "
					                                    + result.GenericEvent.GetType().ToShortTypeName());
				}

				_events.Add(result.GenericEvent, result);
			}
		}
	}
}