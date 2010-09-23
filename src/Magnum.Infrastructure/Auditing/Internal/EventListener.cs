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
	using System.Security.Principal;
	using System.Threading;
	using Collections;
	using Extensions;
	using Magnum.Channels;
	using NHibernate.Event;
	using Reflection;


	public abstract class EventListener<TEvent>
		where TEvent : AbstractEvent
	{
		readonly UntypedChannel _channel;
		readonly Cache<Type, Action<TEvent>> _dispatchers;
		readonly HashSet<Type> _types;

		protected EventListener(UntypedChannel channel, HashSet<Type> types)
		{
			_dispatchers = new Cache<Type, Action<TEvent>>(GetDispatchAction);
			_channel = channel;
			_types = types;
		}

		protected abstract Type GetDispatchKey(TEvent e);

		protected void OnEvent(TEvent e)
		{
			_dispatchers[GetDispatchKey(e)](e);
		}

		Action<TEvent> GetDispatchAction(Type type)
		{
			return this.FastInvoke<EventListener<TEvent>, Action<TEvent>>(new[] {type}, "CreateDispatchAction");
		}

		Action<TEvent> CreateDispatchAction<T>()
		{
			if (!_types.Contains(typeof(T)))
				return x => { };

			return SendEvent<T>;
		}

		protected void Send<T>(T message)
		{
			_channel.Send(message);
		}

		protected abstract void SendEvent<T>(TEvent e);

		protected static IIdentity GetIdentity()
		{
			IPrincipal principal = Thread.CurrentPrincipal;
			if (principal != null)
			{
				IIdentity identity = principal.Identity;
				if (identity != null)
				{
					if (identity.IsAuthenticated && identity.Name.IsNotEmpty())
						return identity;
				}
			}

			return WindowsIdentity.GetCurrent();
		}

		protected T SetGenericEventProperties<T>(T auditEvent, IEventSource session)
			where T : AuditEventImpl
		{
			auditEvent.SessionId = session.SessionId;
			auditEvent.Timestamp = session.Timestamp;
			auditEvent.Identity = EventListener<PostInsertEvent>.GetIdentity();

			return auditEvent;
		}
	}
}