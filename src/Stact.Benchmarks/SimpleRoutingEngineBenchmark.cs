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
	using System.Diagnostics;
	using Magnum.Concurrency;
	using Magnum.Extensions;


	public class SimpleRoutingEngineBenchmark
	{
		public void Run()
		{
			var messageA = new A();

			RunTestCase<A, A>((count, future) => SameMessageTypeTest<A, A>(count, future, () => messageA));
			RunTestCase<A, Message<A>>((count, future) => SameMessageTypeTest<A, A>(count, future, () => messageA));
			RunTestCase<Message<A>, A>((count, future) => SameMessageTypeTest<A, A>(count, future, () => messageA));
		}

		void RunTestCase<TInput, TConsumer>(Func<int, Future<int>, bool> testCase)
		{
			var warmup = new Future<int>();
			testCase(10, warmup);

			Stopwatch timer = Stopwatch.StartNew();

			const int messageCount = 20000;

			var complete = new Future<int>();
			bool completed = testCase(messageCount, complete);

			timer.Stop();

			long elapsedMS = Math.Max(timer.ElapsedMilliseconds, 1);

			if (!completed)
			{
				Console.WriteLine("Process did not complete");
				return;
			}

			Console.WriteLine("Routing {0} to {1} Benchmark", typeof(TInput).ToShortTypeName(),
			                  typeof(TConsumer).ToShortTypeName());

			Console.WriteLine("Processed {0} messages in {1}ms", (long)messageCount, elapsedMS);

			Console.WriteLine("That's {0} messages per second!", (messageCount*1000L)/elapsedMS);
		}


		bool SameMessageTypeTest<TInput, TConsumer>(int messageCount, Future<int> complete, Func<TInput> messageProvider)
		{
			var latch = new CountdownLatch(messageCount, complete.Complete);

			//ActorFactory<TestActor<TConsumer>> factory = ActorFactory<>.Create(inbox => new TestActor<TConsumer>(inbox, latch));
		    ActorRef actor = null;// factory.New();

			for (int i = 0; i < messageCount; i++)
				actor.Send(messageProvider().ToMessage());

			bool completed = complete.WaitUntilCompleted(30.Seconds());

			actor.Exit();

			return completed;
		}


		class A {}


		class TestActor<TConsumer>
		{
			readonly CountdownLatch _latch;

			public TestActor(Actor<TestActor<TConsumer>>  inbox, CountdownLatch latch)
			{
				_latch = latch;
				inbox.Loop(loop =>
				{
					loop.Receive<TConsumer>(m =>
					{
						_latch.CountDown();
						loop.Continue();
					});
				});
			}
		}
	}
}