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
namespace Stact.Configuration.Internal
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Magnum.Extensions;
	using Magnum.Reflection;
	using Stact.Internal;


	public class PropertyChannelsConvention<TActor> :
		ActorConvention<TActor>
		where TActor : Actor
	{
		ActorConvention<TActor>[] _methods;

		public PropertyChannelsConvention()
		{
			_methods = typeof(TActor)
				.GetProperties(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => x.PropertyType.Implements(typeof(Channel<>)))
				.Select(CreateMethodConvention)
				.ToArray();
		}

		public void Initialize(TActor instance, Fiber fiber, Scheduler scheduler, Inbox inbox)
		{
			_methods.Each(method => method.Initialize(instance, fiber, scheduler, inbox));
		}

		public bool Matches(ActorConvention<TActor> convention)
		{
			return typeof(PropertyChannelsConvention<TActor>).Equals(convention.GetType());
		}

		static ActorConvention<TActor> CreateMethodConvention(PropertyInfo property)
		{
			Type messageType = property.PropertyType.GetGenericTypeDeclarations(typeof(Channel<>)).Single();

			var genericTypes = new[] {typeof(TActor), messageType};

			var args = new object[] {property};

			return (ActorConvention<TActor>)FastActivator.Create(typeof(PropertyChannelConvention<,>), genericTypes, args);
		}
	}
}