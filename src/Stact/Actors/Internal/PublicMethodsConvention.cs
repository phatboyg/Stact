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
namespace Stact.Internal
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Magnum.Extensions;
	using Magnum.Reflection;


	public class PublicMethodsConvention<TActor> :
		ActorConvention<TActor>
		where TActor : Actor
	{
		readonly ActorConvention<TActor>[] _methods;

		public PublicMethodsConvention()
		{
			_methods = typeof(TActor)
				.GetMethods(BindingFlags.Instance | BindingFlags.Public)
				.Where(x => x.GetParameters().Count() == 1)
				.Where(x => x.GetParameters().Single().ParameterType.Implements(typeof(Message<>)))
				.Select(CreateMethodConvention)
				.ToArray();
		}

		public void Initialize(TActor instance, Fiber fiber, Scheduler scheduler, Inbox inbox)
		{
			_methods.Each(method => method.Initialize(instance, fiber, scheduler, inbox));
		}

		static ActorConvention<TActor> CreateMethodConvention(MethodInfo method)
		{
			Type messageType = method.GetParameters().Single().ParameterType;

			var genericTypes = new[] {typeof(TActor), messageType};

			var args = new object[] {method};

			return (ActorConvention<TActor>)FastActivator.Create(typeof(PublicMethodConvention<,>), genericTypes, args);
		}
	}
}