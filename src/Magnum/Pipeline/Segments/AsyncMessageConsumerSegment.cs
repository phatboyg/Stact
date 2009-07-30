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
namespace Magnum.Pipeline.Segments
{
	using System.Collections.Generic;
	using Actors;
	using Actors.CommandQueues;

	public class AsyncMessageConsumerSegment<TMessage> :
		MessageConsumerSegment
		where TMessage : class
	{
		private readonly MessageConsumer<TMessage> _consumer;
		private readonly CommandQueue _commandQueue = new ThreadPoolCommandQueue();

		public AsyncMessageConsumerSegment(MessageConsumer<TMessage> consumer)
			: base(typeof (TMessage))
		{
			_consumer = consumer;
		}

		public override IEnumerable<MessageConsumer<T>> Accept<T>(T message)
		{
			TMessage msg = message as TMessage;
			if (msg != null)
			{
				yield return x => _commandQueue.Enqueue(() => _consumer(x as TMessage));
			}
		}
	}
}