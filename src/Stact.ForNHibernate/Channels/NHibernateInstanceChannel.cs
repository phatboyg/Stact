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
namespace Stact.ForNHibernate.Channels
{
	using Fibers;
	using Stact.Channels;
	using NHibernate;


	/// <summary>
	/// A channel that sends a message to a persistent instance that implements
	/// the channel
	/// </summary>
	/// <typeparam name="TInstance">The class type stored in the database</typeparam>
	/// <typeparam name="TChannel">The channel type of the message</typeparam>
	/// <typeparam name="TKey">The key type used by the message and the class</typeparam>
	public class NHibernateInstanceChannel<TInstance, TChannel, TKey> :
		Channel<TChannel>
		where TInstance : class
	{
		readonly ChannelAccessor<TInstance, TChannel> _channelAccessor;
		readonly InstanceChannelPolicy<TInstance, TChannel> _channelPolicy;
		readonly Fiber _fiber;
		readonly KeyAccessor<TChannel, TKey> _messageKeyAccessor;
		readonly SessionProvider<TChannel> _sessionProvider;

		public NHibernateInstanceChannel(Fiber fiber,
		                                 SessionProvider<TChannel> sessionProvider,
		                                 KeyAccessor<TChannel, TKey> messageKeyAccessor,
		                                 ChannelAccessor<TInstance, TChannel> channelAccessor,
		                                 InstanceChannelPolicy<TInstance, TChannel> channelPolicy)
		{
			_fiber = fiber;
			_sessionProvider = sessionProvider;
			_channelPolicy = channelPolicy;
			_channelAccessor = channelAccessor;
			_messageKeyAccessor = messageKeyAccessor;
		}

		public void Send(TChannel message)
		{
			_fiber.Add(() => HandleSend(message));
		}

		void HandleSend(TChannel message)
		{
			using (ISession session = _sessionProvider(message))
			using (ITransaction transaction = session.BeginTransaction())
			{
				TKey key = _messageKeyAccessor(message);

				var instance = session.Get<TInstance>(key, LockMode.Upgrade);
				if (instance != null)
					SendToExistingInstance(instance, message);
				else
				{
					instance = SendToNewInstance(message);
					if (instance != null)
						session.Save(instance, key);
				}

				transaction.Commit();
			}
		}

		TInstance SendToNewInstance(TChannel message)
		{
			TInstance instance;
			if (_channelPolicy.CanCreateInstance(message, out instance))
			{
				SendToInstanceChannel(instance, message);
			}
			else
				_channelPolicy.WasNotHandled(message);

			return instance;
		}

		void SendToExistingInstance(TInstance instance, TChannel message)
		{
			if (_channelPolicy.IsHandledByExistingInstance(message))
				SendToInstanceChannel(instance, message);
			else
				_channelPolicy.WasNotHandled(message);
		}

		void SendToInstanceChannel(TInstance instance, TChannel message)
		{
			Channel<TChannel> channel = _channelAccessor(instance);

			channel.Send(message);
		}
	}
}