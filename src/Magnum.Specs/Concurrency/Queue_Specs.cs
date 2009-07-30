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
namespace Magnum.Specs.Concurrency
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using Magnum.Concurrency;
	using NUnit.Framework;

	[TestFixture]
	public class Queue_Specs
	{
		private ConcurrentQueue<TestMessage> _queue;
		private Queue<TestMessage> _basicQueue;
		private long _limit;
		private long _read;
		private Stopwatch _timer;

		[SetUp]
		public void Setup()
		{
			_queue = new ConcurrentQueue<TestMessage>();
			_basicQueue = new Queue<TestMessage>();
			_limit = 5000000;

			_timer = Stopwatch.StartNew();
		}

		[TearDown]
		public void Teardown()
		{
			_timer.Stop();

			Trace.WriteLine("Messages Received: " + _read);
			Trace.WriteLine("Time to complete " + _limit + " messages = " + _timer.ElapsedMilliseconds + "ms");
			Trace.WriteLine("Messages per second = " + (_limit * 1000) / _timer.ElapsedMilliseconds);
		}

		public class TestMessage
		{
			public int Index { get; set; }

			public TestMessage(int index)
			{
				Index = index;
			}
		}

		[Test]
		public void Test_the_queue()
		{
			Thread pusherThread = new Thread(Pusher);
			pusherThread.Start();
			Thread pusherThread2 = new Thread(Pusher);
			pusherThread2.Start();

			Thread popperThread = new Thread(Popper);
			popperThread.Start();
			Thread popperThread2 = new Thread(Popper);
			popperThread2.Start();

			popperThread.Join(10000);
			popperThread2.Join(1000);
		}

		[Test]
		public void Test_the_basic_queue()
		{
			Thread pusherThread = new Thread(BasicPusher);
			pusherThread.Start();
			Thread pusherThread2 = new Thread(BasicPusher);
			pusherThread2.Start();

			Thread popperThread = new Thread(BasicPopper);
			popperThread.Start();
			Thread popperThread2 = new Thread(BasicPopper);
			popperThread2.Start();


			popperThread.Join(10000);
			popperThread2.Join(1000);

		}

		private void Pusher()
		{
			for (int i = 0; i < _limit / 2; i++)
			{
				var message = new TestMessage(i);
				_queue.Enqueue(message);
			}
		}

		private void Popper()
		{
			while ( _read < _limit)
			{
				TestMessage message;
				if (_queue.Dequeue(out message))
				{
					Interlocked.Increment(ref _read);
				}
			}
		}

		private void BasicPusher()
		{
			for (int i = 0; i < _limit / 2; i++)
			{
				var message = new TestMessage(i);
				lock(_basicQueue)
					_basicQueue.Enqueue(message);
			}
		}

		private void BasicPopper()
		{
			while ( _read < _limit)
			{
				try
				{
					TestMessage message;
					lock(_basicQueue)
						message = _basicQueue.Dequeue();
					if(message != null)
					{
						Interlocked.Increment(ref _read);
					}
				}
				catch (InvalidOperationException)
				{
				}
			}
		}
	}
}