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
namespace Stact.Channels
{
	using System.Collections.Generic;
	using Magnum.Extensions;
	using Fibers;
	using Internal;


	/// <summary>
	/// A channel that converts a collection of messages into a dictionary of distinct
	/// messages by the specified key
	/// </summary>
	/// <typeparam name="T">The type of message delivered on the channel</typeparam>
	/// <typeparam name="TKey">The type of the key for the message</typeparam>
	public class DistinctChannel<T, TKey> :
		Channel<ICollection<T>>
	{
		readonly Fiber _fiber;
		readonly KeyAccessor<T, TKey> _keyAccessor;

		/// <summary>
		/// Constructs a channel
		/// </summary>
		/// <param name="fiber">The queue where consumer actions should be enqueued</param>
		/// <param name="keyAccessor">Returns the key for the message</param>
		/// <param name="output">The method to call when a message is sent to the channel</param>
		public DistinctChannel(Fiber fiber, KeyAccessor<T, TKey> keyAccessor, Channel<IDictionary<TKey, T>> output)
		{
			_fiber = fiber;
			_keyAccessor = keyAccessor;
			Output = output;
		}

		public Channel<IDictionary<TKey, T>> Output { get; private set; }

		public void Send(ICollection<T> message)
		{
			_fiber.Add(() => SendMessagesToOutputChannel(message));
		}

		void SendMessagesToOutputChannel(IEnumerable<T> messages)
		{
			MessageDictionary<TKey, T> result = new MessageDictionaryImpl<TKey, T>(_keyAccessor);

			messages.Each(result.Add);

			Output.Send(result.RemoveAll());
		}
	}
}