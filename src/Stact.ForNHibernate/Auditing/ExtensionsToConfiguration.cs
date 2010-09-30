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
namespace Stact.ForNHibernate.Auditing
{
	using Extensions;
	using Internal;
	using Stact.Channels;
	using NHibernate.Cfg;


	public static class ExtensionsToConfiguration
	{
		/// <summary>
		/// Examines the channel network and identifies any audit event consumers and connects
		/// the appropriate event listeners to the NHibernate configuration so that events are
		/// sent from NHibernate to the channel network
		/// </summary>
		/// <param name="cfg">The NHibernate configuration being modified</param>
		/// <param name="channel">The channel containing the audit event listeners</param>
		public static void AddAuditEventListeners(this Configuration cfg, UntypedChannel channel)
		{
			var configurators = new EventListenerConfigurator[]
				{
					new PreInsertEventConfigurator(),
					new PostInsertEventConfigurator(),
					new PreUpdateEventConfigurator(),
					new PostUpdateEventConfigurator(),
				};

			var visitor = new AuditEventConsumerChannelVisitor(configurators);
			visitor.Configure(channel);

			configurators.Each(x => x.ApplyTo(cfg, channel));
		}
	}
}