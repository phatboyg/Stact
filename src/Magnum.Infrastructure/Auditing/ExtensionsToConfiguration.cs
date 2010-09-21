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
namespace Magnum.Infrastructure.Auditing
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Internal;
	using Magnum.Channels;
	using NHibernate.Cfg;
	using NHibernate.Event;


	public static class ExtensionsToConfiguration
	{
		public static void AddAuditEventListeners(this Configuration cfg, UntypedChannel channel)
		{
			var visitor = new AuditEventConsumerChannelVisitor();

			visitor.GetAuditEvents(channel);

			cfg.AddPreUpdateListener(channel, visitor.PreUpdateTypes);
			cfg.AddPostUpdateListener(channel, visitor.PostUpdateTypes);
		}

		static void AddPreUpdateListener(this Configuration cfg, UntypedChannel channel, IEnumerable<Type> preUpdateTypes)
		{
			var types = new HashSet<Type>(preUpdateTypes);
			if (types.Count == 0)
				return;

			IPreUpdateEventListener listener = new PreUpdateListener(channel, types);

			cfg.EventListeners.PreUpdateEventListeners = cfg.EventListeners.PreUpdateEventListeners == null
			                                             	? new[] {listener}
			                                             	: cfg.EventListeners.PreUpdateEventListeners
			                                             	  	.Concat(Enumerable.Repeat(listener, 1))
			                                             	  	.ToArray();
		}

		static void AddPostUpdateListener(this Configuration cfg, UntypedChannel channel, IEnumerable<Type> postUpdateTypes)
		{
			var types = new HashSet<Type>(postUpdateTypes);
			if (types.Count == 0)
				return;

			IPostUpdateEventListener listener = new PostUpdateListener(channel, types);

			cfg.EventListeners.PostUpdateEventListeners = cfg.EventListeners.PostUpdateEventListeners == null
			                                              	? new[] {listener}
			                                              	: cfg.EventListeners.PostUpdateEventListeners
			                                              	  	.Concat(Enumerable.Repeat(listener, 1))
			                                              	  	.ToArray();
		}
	}
}