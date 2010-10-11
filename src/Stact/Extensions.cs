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
	
	using Internal;
	using Magnum.Extensions;
	using Magnum.Reflection;


	public static class Extensions
	{
		public static Uri ToMessageUrn(this Type type)
		{
			return new Uri("urn:message:" + type.FullName.Replace(".", ":"));
		}

		/// <summary>
		///   Sends an Exit message to an actor instance without waiting for a response
		/// </summary>
		/// <param name = "instance">The actor instance</param>
		public static void Exit(this ActorInstance instance)
		{
			instance.Send<Exit>(new ExitImpl());
		}

		/// <summary>
		///   Sends an Exit message to an actor instance
		/// </summary>
		/// <param name = "instance">The actor instance</param>
		/// <param name = "sender">The exit request sender</param>
		public static SentRequest<Exit> Exit(this ActorInstance instance, Inbox sender)
		{
			return instance.Request<Exit>(new ExitImpl(), sender);
		}

		/// <summary>
		///   Sends a Kill message to an actor instance
		/// </summary>
		/// <param name = "instance">The actor instance</param>
		public static void Kill(this ActorInstance instance)
		{
			instance.Send<Kill>(new KillImpl());
		}

		/// <summary>
		/// Initializes an actor, adding the appropriate channels, etc.
		/// 
		/// This is used since the final initializer that is generated is cached for performance
		/// </summary>
		/// <typeparam name="TActor">The actor type to initialize</typeparam>
		/// <param name="actor">The actor instance to initialize</param>
		/// <param name="fiber">The </param>
		/// <param name="initializer">The initializer method, which is cached per actor type</param>
		public static void Bind<TActor>(this TActor actor, Fiber fiber, Action<ActorInitializer<TActor>> initializer)
		{
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