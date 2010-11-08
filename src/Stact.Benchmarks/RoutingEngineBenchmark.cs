namespace Stact.Benchmarks
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using Internal;
	using Magnum.Concurrency;
	using Magnum.Extensions;
	using Routing;
	using Routing.Visualizers;


	public class RoutingEngineBenchmark
	{
		class A
		{
		}

		class B
		{
		}

		public void Run()
		{
			Run(() => new A(), () => new B());
			Run(() => new A(), () => new B());
		}

		void Run<T1, T2>(Func<T1> value1Provider, Func<T2> value2Provider)
		{
			Stopwatch timer = Stopwatch.StartNew();

			const int consumerCount = 10;
			const int messageCount = 2000;

			var complete = new Future<int>();

			int totalMessageCount;
			bool completed = RunTest(consumerCount, messageCount, complete, value1Provider, value2Provider, out totalMessageCount);

			timer.Stop();

			if (!completed)
			{
				Console.WriteLine("Process did not complete");
				return;
			}

			Console.WriteLine("Channel<{0}> Benchmark", typeof(T1).Name);

			Console.WriteLine("Processed {0} messages with {1} consumers in {2}ms", totalMessageCount, consumerCount,
			                  timer.ElapsedMilliseconds);

			Console.WriteLine("That's {0}K messages per second!", totalMessageCount / timer.ElapsedMilliseconds);
		}

		bool RunTest<T1,T2>(int consumerCount, int messageCount, Future<int> complete, Func<T1> value1Provider, Func<T2> value2Provider, out int totalMessageCount)
		{
			var engine = new DynamicRoutingEngine(new PoolFiber());

			int joinCount = (messageCount/2)*(messageCount/2)*consumerCount;
			totalMessageCount = consumerCount*messageCount + joinCount;
			var latch = new CountdownLatch(totalMessageCount, complete.Complete);

			int countA = 0;
			int countB = 0;
			int countC = 0;
			for (int i = 0; i < consumerCount; i++)
			{
				engine.Receive<A>(m =>
					{
						Interlocked.Increment(ref countA);
						latch.CountDown();
					});
				engine.Receive<B>(m =>
					{
						Interlocked.Increment(ref countB);
						latch.CountDown();
					});
				engine.Receive<A, B>(m =>
					{
						Interlocked.Increment(ref countC);
						latch.CountDown();
					});
			}

			//var visualizer = new RoutingEngineTextVisualizer();
			//visualizer.Output = s => Console.WriteLine(s);
			//visualizer.Visit(engine);

			for (int i = 0; i < messageCount; i += 2)
			{
				engine.Send(value1Provider());
				engine.Send(value2Provider());
			}

			bool waitUntilCompleted = complete.WaitUntilCompleted(30.Seconds());

				Console.WriteLine("Consumed A: " + countA);
				Console.WriteLine("Consumed B: " + countB);
				Console.WriteLine("Consumed C: " + countC);


			return waitUntilCompleted;
		}
	}
}