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
	using System.Linq.Expressions;
	using System.Reflection;


	/// <summary>
	/// Connects an actor property that implements Channel&lt;T&gt; to the inbox
	/// </summary>
	/// <typeparam name="TActor">The actor type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public class PropertyChannelConvention<TActor, TChannel> :
		ActorConvention<TActor>
		where TActor : class, Actor
	{
		readonly ChannelAccessor<TActor, TChannel> _channelAccessor;

		public PropertyChannelConvention(PropertyInfo property)
		{
			_channelAccessor = GenerateChannelAccessor(property);
		}

		public void Initialize(TActor instance, Fiber fiber, Scheduler scheduler, Inbox inbox)
		{
			Channel<TChannel> channel = _channelAccessor(instance);
			if (channel == null)
				return;

			inbox.Repeat().Receive<TChannel>(x => channel.Send(x));
		}

		public bool Matches(ActorConvention<TActor> convention)
		{
			return typeof(PropertyChannelConvention<TActor, TChannel>).Equals(convention.GetType());
		}

		static ChannelAccessor<TActor, TChannel> GenerateChannelAccessor(PropertyInfo property)
		{
			ParameterExpression target = Expression.Parameter(typeof(TActor), "x");
			MethodCallExpression getter = Expression.Call(target, property.GetGetMethod(true));

			return Expression.Lambda<ChannelAccessor<TActor, TChannel>>(getter, target).Compile();
		}
	}
}