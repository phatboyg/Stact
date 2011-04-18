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
namespace Stact
{
	public static class ExtensionsForLinking
	{
		/// <summary>
		/// Initiates a link to another actor instance. Once linked, if either actor
		/// exits, all actors linked to this actor will be sent an Exit with a reason
		/// of actor death
		/// </summary>
		/// <param name="actor">The actor to link</param>
		/// <param name="inbox">The inbox of the actor requesting the link</param>
		public static SentRequest<Link> Link(this ActorInstance actor, Inbox inbox)
		{
			return actor.Request<Link>(inbox);
		}
	}
}