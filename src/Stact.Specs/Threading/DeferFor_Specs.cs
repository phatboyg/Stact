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
namespace Stact.Specs.Threading
{
    using System;
    using System.Threading;
    using Stact.Extensions;
    using NUnit.Framework;

    [TestFixture]
    public class DeferFor_Specs
    {
        private ManualResetEvent _event;

        [SetUp]
        public void Setup()
        {
            _event = new ManualResetEvent(false);
        }

        [Test]
        public void The_method_should_be_called_when_the_defer_time_is_elapsed()
        {
            Action asyncAction = () => _event.Set();
            asyncAction.DeferFor(2.Seconds());

            Assert.IsFalse(_event.WaitOne(1.Seconds(), true), "should not have been set yet");

            Assert.IsTrue(_event.WaitOne(3.Seconds(), true), "Timeout waiting for event to be set");
        }

    }

    public static class ActionExtensions
    {
        public static void DeferFor(this Action action, TimeSpan timeSpan)
        {
            ThreadStart scheduledAction = () =>
                                              {
                                                  Thread.Sleep(timeSpan);
                                                  action();
                                              };

            Thread thread = new Thread(scheduledAction);
            thread.IsBackground = true;
            thread.Start();
        }
    }
}