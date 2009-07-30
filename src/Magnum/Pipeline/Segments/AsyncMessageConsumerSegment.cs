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
	using System;
	using System.Collections.Generic;
	using Actors;
	using Actors.CommandQueues;

	public abstract class AsyncMessageConsumerSegment :
		PipeSegment
	{
		protected AsyncMessageConsumerSegment(Type messageType, Type consumerType)
			: base(PipeSegmentType.AsyncMessageConsumer, messageType)
		{
			ConsumerType = consumerType;
		}

		public Type ConsumerType { get; set; }
	}

	public class AsyncMessageConsumerSegment<TConsumer, TMessage> :
		AsyncMessageConsumerSegment
		where TMessage : class
		where TConsumer : IAsyncConsumer<TMessage>
	{
		private readonly Func<TConsumer> _getConsumer;
		private readonly CommandQueue _commandQueue = new ThreadPoolCommandQueue();

		public AsyncMessageConsumerSegment(TConsumer consumer)
			: base(typeof (TMessage), typeof(TConsumer))
		{
			_getConsumer = () => consumer;
		}

		public AsyncMessageConsumerSegment(Func<TConsumer> getConsumer)
			: base(typeof (TMessage), typeof(TConsumer))
		{
			_getConsumer = getConsumer;
		}

		public override IEnumerable<MessageConsumer<T>> Accept<T>(T message)
		{
			TMessage msg = message as TMessage;
			if (msg != null)
			{
				yield return x => _commandQueue.Enqueue(() => _getConsumer().Consume(x as TMessage));
			}
		}
	}
}