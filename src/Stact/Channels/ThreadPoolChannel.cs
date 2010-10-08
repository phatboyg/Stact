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
	using System.Threading;
	using Magnum;


	/// <summary>
	/// Keeps a fixed number of channels available, which presumably are doing some form of synchronous processing
	/// of messages, to avoid too many consumers running at the same time
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public class ThreadPoolChannel<T> :
		Channel<T>
	{
		readonly int _channelLimit;
		readonly object _lock = new object();
		int _channelCount;

		public ThreadPoolChannel(ChannelProvider<T> instanceProvider, int channelLimit)
		{
			Guard.GreaterThan(0, channelLimit, "channelLimit");

			_channelLimit = channelLimit;
			_channelCount = 0;
			InstanceProvider = instanceProvider;
		}

		public ChannelProvider<T> InstanceProvider { get; private set; }

		public void Send(T message)
		{
			lock (_lock)
			{
				while (_channelCount >= _channelLimit)
					Monitor.Wait(_lock);

				_channelCount++;

				ThreadPool.QueueUserWorkItem(x => SendMessageToChannel(message));
			}
		}

		void SendMessageToChannel(T message)
		{
			try
			{
				Channel<T> channel = InstanceProvider.GetChannel(message);

				channel.Send(message);
			}
			finally
			{
				lock (_lock)
				{
					_channelCount--;
					Monitor.Pulse(_lock);
				}
			}
		}
	}
}