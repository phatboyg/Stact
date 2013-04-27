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
    using System.Threading;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;


    [TestFixture]
    public class Running_all_actions_using_a_thread_queue
    {
        [Test]
        [Category("Slow")]
        public void Should_result_in_no_waiting_actions_in_the_queue()
        {
            Fiber fiber = new ThreadFiber();

            fiber.Execute(() => Thread.Sleep(1000));

            var called = new Future<bool>();

            fiber.Execute(() => called.Complete(true));

            fiber.Stop(112.Seconds());

            called.IsCompleted.ShouldBeTrue();
        }
    }
}