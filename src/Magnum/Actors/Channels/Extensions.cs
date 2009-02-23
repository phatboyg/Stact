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
namespace Magnum.Actors.Channels
{
	using System;
	using System.Collections.Generic;
	using Subscribers;

	public static class Extensions
	{
		public static Unsubscribe Subscribe<T>(this Channel<T> channel, CommandQueue queue, Action<T> consume)
		{
			return channel.Subscribe(message => queue.Enqueue(() => consume(message)));
		}

		public static Unsubscribe Subscribe<T>(this Channel<T> channel, Action<T> consume, int interval, Scheduler scheduler)
		{
			var subscriber = new LatestIntervalSubscriber<T>(consume, interval, scheduler);
			return channel.Subscribe(subscriber.Consume);
		}

		public static Unsubscribe Subscribe<T>(this Channel<T> channel, CommandQueue queue, Action<T> consume, Filter<T> filter)
		{
			var subscriber = new FilteredSubscriber<T>(queue, consume, filter);
			return channel.Subscribe(subscriber.Consume);
		}

		public static Unsubscribe Subscribe<T>(this Channel<T> channel, Action<IList<T>> consume, int interval, Scheduler scheduler)
		{
			var subscriber = new IntervalSubscriber<T>(consume, interval, scheduler);
			return channel.Subscribe(subscriber.Consume);
		}

		public static Unsubscribe Subscribe<T, K>(this Channel<T> channel, Action<IDictionary<K, T>> consume, int interval, Converter<T, K> keyConverter, Scheduler scheduler)
		{
			var subscriber = new DistinctIntervalSubscriber<K, T>(consume, interval, keyConverter, scheduler);
			return channel.Subscribe(subscriber.Consume);
		}
	}
}