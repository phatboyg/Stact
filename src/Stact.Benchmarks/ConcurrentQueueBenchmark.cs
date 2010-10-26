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
namespace Stact.Benchmarks
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;


	public class ConcurrentQueueBenchmark
	{
		public void Run()
		{
			int producerCount = 100;
			int consumerCount = 100;
			int iterations = 10000000/producerCount;

			Console.WriteLine("using regular queue");

			UsingQueue(3, (producer, consumer) =>
				{
					//
					Loop(producer, consumer, producerCount, consumerCount, iterations);
				});

			Console.WriteLine("using concurrent queue");
			UsingConcurrentQueue(3,(producer, consumer) =>
				{
					//
					Loop(producer, consumer, producerCount, consumerCount, iterations);
				});

			Console.WriteLine("using array list");
			UsingList(3,(producer, consumer) =>
				{
					//
					Loop(producer, consumer, producerCount, consumerCount, iterations);
				});
		}

		void UsingQueue(int loops, Action<Action<string>, Action<Action<string>, Action>> testCallback)
		{
			var queue = new Queue<string>();
			var obj = new object();

			Action<string> producer = x =>
				{
					lock (obj)
					{
						queue.Enqueue(x);
					}
				};

			Action<Action<string>, Action> consumer = (handle, empty) =>
				{
					lock (obj)
					{
						if (queue.Count > 0)
						{
							handle(queue.Dequeue());
							return;
						}
					}

					empty();
				};

			for (int i = 0; i < loops; i++)
				testCallback(producer, consumer);
		}

		void UsingList(int loops, Action<Action<string>, Action<Action<string>, Action>> testCallback)
		{
			var queue = new List<string>();
			var obj = new object();

			Action<string> producer = x =>
				{
					lock (obj)
					{
						queue.Add(x);
					}
				};

			Action<Action<string>, Action> consumer = (handle, empty) =>
				{
					string[] values = null;

					lock (obj)
					{
						if (queue.Count > 0)
						{
							values = queue.ToArray();
							queue.Clear();
						}
					}

					if(values == null)
						empty();
					else
					{
						for (int i = 0; i < values.Length; i++)
						{
							handle(values[i]);
						}
					}
				};

			for (int i = 0; i < loops; i++)
				testCallback(producer, consumer);
		}

		void UsingConcurrentQueue(int loops, Action<Action<string>, Action<Action<string>, Action>> testCallback)
		{
			var queue = new ConcurrentQueue<string>();

			Action<string> producer = queue.Enqueue;

			Action<Action<string>, Action> consumer = (handle, empty) =>
				{
					string result;
					if (queue.TryDequeue(out result))
						handle(result);
					else
						empty();
				};

			for (int i = 0; i < loops; i++)
				testCallback(producer, consumer);
		}

		void Loop(Action<string> producer, Action<Action<string>, Action> consumer, int producerCount, int consumerCount,
		          int iterations)
		{
			var producers = new Producer[producerCount];
			var consumers = new Consumer[consumerCount];

			int remaining = iterations*producerCount;

			Stopwatch timer = Stopwatch.StartNew();

			for (int i = 0; i < consumerCount; i++)
			{
				consumers[i] = new Consumer(consumer, () => remaining == 0, () => Interlocked.Decrement(ref remaining));
				consumers[i].Start();
			}

			for (int i = 0; i < producerCount; i++)
			{
				producers[i] = new Producer(producer, iterations);
				producers[i].Start();
			}

			for (int i = 0; i < producerCount; i++)
				producers[i].Stop();

			for (int i = 0; i < consumerCount; i++)
				consumers[i].Stop();

			timer.Stop();

			Console.WriteLine("Total Time: " + timer.ElapsedMilliseconds + "ms");
		}


		class Consumer :
			Worker
		{
			Action<Action<string>, Action> _consumeDelegate;
			Func<bool> _done;
			Action _step;

			public Consumer(Action<Action<string>, Action> consumeDelegate, Func<bool> done, Action step)
			{
				_consumeDelegate = consumeDelegate;
				_step = step;
				_done = done;
			}

			protected override void RunThread()
			{
				bool processing = true;

				while (processing)
				{
					_consumeDelegate(result =>
						{
							long value = 1;
							for (int i = 0; i < 100; i++)
								value = value + value + 1;

							if (result.Length < value)
								result = null;

							_step();
						}, () =>
							{
								if (_done())
									processing = false;
								else
									Thread.Sleep(10);
							});
				}
			}
		}


		class Producer :
			Worker
		{
			int _iterations;

			Action<string> _produceDelegate;

			public Producer(Action<string> produceDelegate, int iterations)
			{
				_produceDelegate = produceDelegate;
				_iterations = iterations;
			}

			protected override void RunThread()
			{
				for (int i = 0; i < _iterations; i++)
					_produceDelegate("Hello");
			}
		}


		abstract class Worker
		{
			Thread _thread;

			public void Start()
			{
				_thread = new Thread(RunThread, 64000);
				_thread.Start();
			}

			public void Stop()
			{
				_thread.Join();
			}

			protected abstract void RunThread();
		}
	}
}