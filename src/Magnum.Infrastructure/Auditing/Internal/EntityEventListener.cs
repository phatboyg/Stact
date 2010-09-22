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
	using Magnum.Channels;
	using NHibernate.Event;
	using NHibernate.Persister.Entity;


	public abstract class EntityEventListener<TEvent> :
		EventListener<TEvent>
		where TEvent : AbstractEvent
	{
		public EntityEventListener(UntypedChannel channel, HashSet<Type> types)
			: base(channel, types)
		{
		}

		protected static IList<PropertyChange> GetChanges(IEntityPersister persister, object[] state)
		{
			IList<PropertyChange> changes = new List<PropertyChange>();

			for (int i = 0; i < state.Length; i++)
			{
				if (state[i] == null)
					continue;

				string propertyName = persister.PropertyNames[i];
				object value = state[i];

				changes.Add(new PropertyChange
					{
						Name = propertyName,
						Value = value,
						OriginalValue = null,
					});

				// consider whether or not we recurse into the entity for additional changes
			}
			return changes;
		}
	}
}