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
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	using Magnum.Extensions;
	using Magnum.Reflection;


	public static class Extensions
	{
		public static MessageUrn ToMessageUrn(this Type type)
		{
			return new MessageUrn(type);
		}

		public static void Start(this ActorInstance instance)
		{
			instance.Send<Start>();
		}

		public static void Stop(this ActorInstance instance)
		{
			instance.Send<Stop>();
		}

		/// <summary>
		///   Sends an Exit message to an actor instance without waiting for a response
		/// </summary>
		/// <param name = "instance">The actor instance</param>
		public static void Exit(this ActorInstance instance)
		{
			instance.Send<Exit>();
		}

		/// <summary>
		///   Sends an Exit message to an actor instance
		/// </summary>
		/// <param name = "instance">The actor instance</param>
		/// <param name = "sender">The exit request sender</param>
		public static SentRequest<Exit> Exit(this ActorInstance instance, Inbox sender)
		{
			return instance.Request<Exit>(sender);
		}

		/// <summary>
		///   Sends a Kill message to an actor instance
		/// </summary>
		/// <param name = "instance">The actor instance</param>
		public static void Kill(this ActorInstance instance)
		{
			instance.Send<Kill>();
		}

		public static void Connect<TActor, TPort>(this TActor actor, Expression<Func<TActor, Channel<TPort>>> portProperty,
		                                          Fiber fiber, Consumer<TPort> consumer)
			where TActor : Actor
		{
			PropertyInfo propertyInfo = portProperty.GetMemberPropertyInfo();
			var property = new FastProperty<TActor, Channel<TPort>>(propertyInfo, BindingFlags.NonPublic);

			var channel = new ConsumerChannel<TPort>(fiber, consumer);

			property.Set(actor, channel);
		}
	}
}