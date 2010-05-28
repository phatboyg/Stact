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
namespace Magnum.Channels
{
	using Fibers;

	/// <summary>
	///   A channel that selectively accepts a message and enqueues the consumer method via the
	///   specified Fiber.
	///   Note that the filter function is called as part of the queued action, so threading
	///   is not an issue.
	/// </summary>
	/// <typeparam name = "T">The type of message delivered on the channel</typeparam>
	public class FilterChannel<T> :
		Channel<T>
	{
		private readonly Fiber _fiber;

		/// <summary>
		///   Constructs a channel
		/// </summary>
		/// <param name = "fiber">The queue where consumer actions should be enqueued</param>
		/// <param name = "output">The method to call when a message is sent to the channel</param>
		/// <param name = "filter">The filter to determine if the message can be consumed</param>
		public FilterChannel(Fiber fiber, Channel<T> output, Filter<T> filter)
		{
			_fiber = fiber;
			Output = output;
			Filter = filter;
		}

		public Filter<T> Filter { get; private set; }

		public Channel<T> Output { get; private set; }

		public void Send(T message)
		{
			_fiber.Add(() =>
				{
					if (Filter(message))
						Output.Send(message);
				});
		}
	}
}