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
namespace Magnum.Specs.Metrics
{
    using System;
    using System.Diagnostics;
    using Common.Metrics.Monitor;
    using MbUnit.Framework;

    [TestFixture]
    public class Counter_Specs
    {
        [Test]
        public void A_counter_should_increment()
        {
            ExecutionMonitor monitor = new ExecutionMonitor(typeof (Counter_Specs), "Test");

            monitor.IncrementStarted();
            monitor.IncrementCompleted();

            Assert.AreEqual(1, monitor.Started);
            Assert.AreEqual(1, monitor.Completed);
            Assert.AreEqual(0, monitor.Failed);
        }

        [Test]
        public void A_counter_should_increment_again()
        {
            CountMonitor monitor = new CountMonitor(typeof (Counter_Specs), "Test");

            monitor.Increment();

            Assert.AreEqual(1, monitor.Count);
        }

        [Test]
        public void A_flow_monitor_should_monitor_the_flow()
        {
            FlowMonitor monitor = new FlowMonitor(typeof (Counter_Specs), "Test");

            monitor.IncrementWrite(64);
            monitor.IncrementWrite(64);
            monitor.IncrementRead(128);
            monitor.IncrementRead(128);

            Assert.AreEqual(2, monitor.WriteCount);
            Assert.AreEqual(128, monitor.BytesWritten);
            Assert.AreEqual(2, monitor.ReadCount);
            Assert.AreEqual(256, monitor.BytesRead);
        }

        [Test]
        public void A_process_monitor_should_get_process_information()
        {
            ProcessMonitor monitor = new ProcessMonitor(typeof (Counter_Specs), "Test");

            Assert.AreEqual(Environment.ProcessorCount, monitor.ProcessorCount);
            //	Assert.AreEqual(Process.GetCurrentProcess().WorkingSet64 >> 20, monitor.MemoryUsed);
            Assert.AreEqual(Process.GetCurrentProcess().Threads.Count, monitor.ThreadCount);
            Assert.GreaterThanOrEqualTo(Process.GetCurrentProcess().TotalProcessorTime, monitor.ProcessorTimeUsed);
        }

        [Test]
        public void The_accumulator_should_propertly_keep_track_of_good_and_evil()
        {
            ExecutionMonitor monitor = new ExecutionMonitor(typeof (Counter_Specs), "Test");

            SuccessRateMonitor aggregate = new SuccessRateMonitor(typeof (Counter_Specs), "Test", monitor);

            monitor.IncrementCompleted();
            monitor.IncrementFailed();

            Assert.AreEqual(50, aggregate.SuccessRate);
        }

        [Test]
        public void The_accumulator_should_propertly_keep_track_of_good_and_evil_round()
        {
            ExecutionMonitor monitor = new ExecutionMonitor(typeof (Counter_Specs), "Test");

            SuccessRateMonitor aggregate = new SuccessRateMonitor(typeof(Counter_Specs), "Test", monitor);

            monitor.IncrementCompleted();
            monitor.IncrementFailed();
            monitor.IncrementFailed();

            Assert.AreEqual(33, aggregate.SuccessRate);
        }
    }
}