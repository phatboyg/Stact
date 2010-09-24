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
namespace Magnum.ForNHibernate.Auditing.Internal
{
	using System;
	using System.Collections.Generic;
	using Magnum.Channels;
	using NHibernate.Event;


	public class PreInsertListener :
		EntityEventListener<PreInsertEvent>,
		IPreInsertEventListener
	{
		public PreInsertListener(UntypedChannel channel, HashSet<Type> types)
			: base(channel, types)
		{
		}

		public bool OnPreInsert(PreInsertEvent @event)
		{
			OnEvent(@event);

			return false;
		}

		protected override Type GetDispatchKey(PreInsertEvent e)
		{
			return e.Entity.GetType();
		}

		protected override void SendEvent<T>(PreInsertEvent e)
		{
			//var persister = e.Persister as IOuterJoinLoadable;
			//string tableName = e.Persister.PropertySpaces[0];

			var entity = (T)e.Entity;
			IList<PropertyChange> changes = GetChanges(e.Persister, e.State);

			PreInsertEventImpl<T> message = SetGenericEventProperties(new PreInsertEventImpl<T>(), e.Session);
			message.Entity = entity;
			message.Changes = changes;

			Send<PreInsertEvent<T>>(message);
		}
	}
}