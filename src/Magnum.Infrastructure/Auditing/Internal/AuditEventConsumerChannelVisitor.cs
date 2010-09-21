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
	using Extensions;
	using Magnum.Channels;
	using Magnum.Channels.Visitors;


	public class AuditEventConsumerChannelVisitor :
		ChannelVisitor
	{
		readonly HashSet<Type> _postUpdateTypes;
		readonly HashSet<Type> _preUpdateTypes;

		public AuditEventConsumerChannelVisitor()
		{
			_preUpdateTypes = new HashSet<Type>();
			_postUpdateTypes = new HashSet<Type>();
		}

		public IEnumerable<Type> PostUpdateTypes
		{
			get { return _postUpdateTypes; }
		}

		public IEnumerable<Type> PreUpdateTypes
		{
			get { return _preUpdateTypes; }
		}

		public void GetAuditEvents<T>(Channel<T> channel)
		{
			Visit(channel);
		}

		public void GetAuditEvents(UntypedChannel channel)
		{
			Visit(channel);
		}

		public override Channel<T> Visit<T>(Channel<T> channel)
		{
			if (typeof(T).Implements<AuditEvent>())
			{
				if (typeof(T).Implements(typeof(PreUpdateEvent<>)))
					_preUpdateTypes.Add(typeof(T).GetGenericTypeDeclarations(typeof(PreUpdateEvent<>)).First());
				else if (typeof(T).Implements(typeof(PostUpdateEvent<>)))
					_postUpdateTypes.Add(typeof(T).GetGenericTypeDeclarations(typeof(PostUpdateEvent<>)).First());
				else
					throw new InvalidOperationException("The audit type is not yet configured: " + typeof(T).ToShortTypeName());
			}

			return base.Visit(channel);
		}
	}
}