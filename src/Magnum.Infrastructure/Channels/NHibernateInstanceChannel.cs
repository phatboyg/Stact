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
namespace Magnum.Infrastructure.Channels
{
	using Fibers;
	using Magnum.Channels;
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
		readonly InstanceProvider<TInstance, TChannel> _instanceProvider;
		readonly KeyAccessor<TChannel, TKey> _messageKeyAccessor;
		readonly Fiber _fiber;
		readonly ISessionFactory _sessionFactory;

		public NHibernateInstanceChannel(Fiber fiber, ISessionFactory sessionFactory, KeyAccessor<TChannel, TKey> messageKeyAccessor,
		                                 ChannelAccessor<TInstance, TChannel> channelAccessor,
		                                 InstanceProvider<TInstance, TChannel> instanceProvider)
		{
			_fiber = fiber;
			_sessionFactory = sessionFactory;
			_instanceProvider = instanceProvider;
			_channelAccessor = channelAccessor;
			_messageKeyAccessor = messageKeyAccessor;
		}

		public void Send(TChannel message)
		{
			_fiber.Add(() => HandleSend(message));
		}

		void HandleSend(TChannel message)
		{
			using (ISession session = _sessionFactory.OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				TKey key = _messageKeyAccessor(message);

				var instance = session.Get<TInstance>(key, LockMode.Upgrade);
				if (instance != null)
					SendToExistingInstance(instance, message);
				else
				{
					instance = SendToNewInstance(message);

					session.Save(instance, key);
				}

				transaction.Commit();
			}
		}

		TInstance SendToNewInstance(TChannel message)
		{
			TInstance instance = _instanceProvider.GetInstance(message);

			SendToInstanceChannel(instance, message);

			return instance;
		}

		void SendToExistingInstance(TInstance instance, TChannel message)
		{
			SendToInstanceChannel(instance, message);
		}

		void SendToInstanceChannel(TInstance instance, TChannel message)
		{
			Channel<TChannel> channel = _channelAccessor(instance);

			channel.Send(message);
		}
	}
}