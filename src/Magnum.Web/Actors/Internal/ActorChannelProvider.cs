// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Web.Actors.Internal
{
	using System.Linq.Expressions;
	using System.Reflection;
	using Channels;

	/// <summary>
	/// Provides a channel from an actor, based on the property information which is used
	/// to create a dynamic method that returns the actual channel
	/// </summary>
	/// <typeparam name="TActor">The actor type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public class ActorChannelProvider<TActor, TChannel> :
		ChannelProvider<TChannel>
		where TActor : class
	{
		private readonly ActorInstanceProvider<TActor> _actorInstanceProvider;
		private readonly ChannelAccessor<TActor, TChannel> _channelAccessor;

		public ActorChannelProvider(ActorInstanceProvider<TActor> actorInstanceProvider, PropertyInfo property)
		{
			Guard.AgainstNull(actorInstanceProvider, "actorInstanceProvider");
			Guard.AgainstNull(property, "property");

			_actorInstanceProvider = actorInstanceProvider;

			_channelAccessor = CreateChannelAccessor(property);
		}

		public Channel<TChannel> GetChannel(TChannel message)
		{
			TActor actor = _actorInstanceProvider.GetActor();

			return _channelAccessor(actor);
		}

		private static ChannelAccessor<TActor, TChannel> CreateChannelAccessor(PropertyInfo property)
		{
			ParameterExpression actor = Expression.Parameter(typeof (TActor), "actor");
			MethodCallExpression getter = Expression.Call(actor, property.GetGetMethod(true));

			return Expression.Lambda<ChannelAccessor<TActor, TChannel>>(getter, new[] {actor})
				.Compile();
		}
	}
}