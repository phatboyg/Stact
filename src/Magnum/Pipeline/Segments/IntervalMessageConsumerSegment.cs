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
	using Actors.Schedulers;

	[Serializable]
	public abstract class IntervalMessageConsumerSegment :
		PipeSegment
	{
		protected IntervalMessageConsumerSegment(Type messageType, Type consumerType)
			: base(PipeSegmentType.IntervalMessageConsumer, messageType)
		{
			ConsumerType = consumerType;
		}

		public Type ConsumerType { get; private set; }
	}

	[Serializable]
	public class IntervalMessageConsumerSegment<TMessage> :
		IntervalMessageConsumerSegment
		where TMessage : class
	{
		[NonSerialized]
		private readonly Func<MessageConsumer<IList<TMessage>>> _getConsumer;

		private readonly int _interval;
		private readonly object _lock = new object();

		[NonSerialized]
		private readonly CommandQueue _commandQueue = new ThreadPoolCommandQueue();

		[NonSerialized]
		private readonly Scheduler _scheduler = new ThreadPoolScheduler();

		[NonSerialized]
		private List<TMessage> _pending;

		public IntervalMessageConsumerSegment(TimeSpan interval, MessageConsumer<IList<TMessage>> consumer)
			: base(typeof (TMessage), typeof (MessageConsumer<IList<TMessage>>))
		{
			_interval = (int) interval.TotalMilliseconds;
			_getConsumer = () => consumer;
		}

		public IntervalMessageConsumerSegment(TimeSpan interval, Type consumerType, IConsumer<IList<TMessage>> consumer)
			: base(typeof (TMessage), consumerType)
		{
			_interval = (int) interval.TotalMilliseconds;
			_getConsumer = () => consumer.Consume;
		}

		public IntervalMessageConsumerSegment(TimeSpan interval, Type consumerType, Func<IConsumer<IList<TMessage>>> getConsumer)
			: base(typeof (TMessage), consumerType)
		{
			_interval = (int) interval.TotalMilliseconds;
			_getConsumer = () => getConsumer().Consume;
		}

		private void DeliverToConsumer()
		{
			if (_pending == null) return;

			IList<TMessage> messages = _pending;
			_pending = null;

			_getConsumer()(messages);
		}

		public override IEnumerable<MessageConsumer<T>> Accept<T>(T message)
		{
			TMessage msg = message as TMessage;
			if (msg != null)
			{
				yield return x => _commandQueue.Enqueue(() =>
					{
						if (_pending == null)
						{
							_pending = new List<TMessage>();
							_scheduler.Schedule(_interval, () => _commandQueue.Enqueue(DeliverToConsumer));
						}

						_pending.Add(x as TMessage);
					});
			}
		}
	}
}