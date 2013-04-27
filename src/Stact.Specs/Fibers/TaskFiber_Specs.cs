// Copyright 2010-2013 Chris Patterson
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
namespace Stact.Specs.Fibers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class TaskFiber_Specs
    {
        [Test]
        public void Should_be_able_to_create_and_shutdown()
        {
            Fiber fiber = new TaskFiber();

            fiber.Stop(TimeSpan.FromSeconds(30));
        }

        [Test]
        public void Should_be_able_to_create_execute_and_shutdown()
        {
            bool done = false;

            Fiber fiber = new TaskFiber();
            fiber.Execute(() => done = true);

            fiber.Stop(TimeSpan.FromSeconds(30));

            Assert.IsTrue(done);
        }

        [Test]
        public void Should_be_able_to_create_execute_and_shutdown_and_not_execute()
        {
            bool done = false;
            bool last = false;

            Fiber fiber = new TaskFiber();
            fiber.Execute(() => done = true);

            fiber.Stop(TimeSpan.Zero);
            fiber.Execute(() => last = true);
            fiber.Stop(TimeSpan.FromSeconds(30));


            Assert.IsTrue(done);
            Assert.IsFalse(last);
        }

        [Test]
        public void Should_not_run_action_on_same_thread()
        {
            var @event = new ManualResetEventSlim(false);
            bool completed = false;

            Fiber fiber = new TaskFiber();
            fiber.Execute(() =>
            {
                completed = @event.Wait(10000);
            });
            @event.Set();
            bool shutdownCompleted = fiber.Stop(TimeSpan.FromSeconds(30));

            Assert.IsTrue(completed);
            Assert.IsTrue(shutdownCompleted);
        }

        [Test]
        public void Should_support_ordered_execution()
        {
            const int count = 100000;
            var values = new int[count];
            Fiber fiber = new TaskFiber();

            int index = 0;
            var completed = new Future<int>();

            var go = new Future<bool>();

            fiber.Execute(() =>
                {
                    go.WaitUntilCompleted(10.Seconds());
                    Console.WriteLine("Started.");
                });

            Stopwatch queueTimer = Stopwatch.StartNew();

            for (int i = 0; i < count; i += 2)
            {
                int offset = i;
                fiber.Execute(() =>
                    {
                        values[offset] = index++;

                        if (offset == count - 1)
                            completed.Complete(offset);
                    }, () =>
                    {
                        values[offset + 1] = index++;

                        if (offset + 1 == count - 1)
                            completed.Complete(offset + 1);
                    });
            }

            queueTimer.Stop();

            Console.WriteLine("Starting...");

            Stopwatch executeTimer = Stopwatch.StartNew();

            go.Complete(true);

            completed.WaitUntilCompleted(10.Seconds()).ShouldBeTrue();

            executeTimer.Stop();

            for (int i = 0; i < count; i++)
            {
                if (values[i] != i)
                    Assert.Fail("Order not correct");
            }
            Console.WriteLine("Queued {0} actions in {1}ms ({2}/s)", count, queueTimer.ElapsedMilliseconds,
                ((long)count * 1000) / queueTimer.ElapsedMilliseconds);
			Console.WriteLine("Executed {0} actions in {1}ms ({2}/s)", count, executeTimer.ElapsedMilliseconds,
                ((long)count * 1000) / executeTimer.ElapsedMilliseconds);
        }
    }

    [TestFixture]
    public class Killing_the_task_fiber
    {
        [Test]
        public void Should_prevent_new_actions_from_being_queued()
        {
            Fiber fiber = new TaskFiber();

            var called = new Future<bool>();

            fiber.Kill();

            fiber.Execute(() => called.Complete(true));

            fiber.Stop(10.Seconds());

            called.IsCompleted.ShouldBeFalse();
        }
    }


    [TestFixture]
    public class Running_all_operations_on_the_task_fiber
    {
        [Test]
        public void Should_result_in_no_waiting_actions_in_the_queue()
        {
            Fiber fiber = new TaskFiber();

            var called = new Future<bool>();

            10.Times(() => fiber.Execute(() => Thread.Sleep(100)));
            fiber.Execute(() => called.Complete(true));

            Stopwatch timer = Stopwatch.StartNew();

            fiber.Stop(8.Seconds());

            timer.Stop();

            called.IsCompleted.ShouldBeTrue();

            timer.ElapsedMilliseconds.ShouldBeLessThan(2000);
        }
    }
}