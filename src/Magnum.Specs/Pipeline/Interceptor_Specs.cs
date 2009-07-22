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
namespace Magnum.Specs.Pipeline
{
    using System;
    using System.Threading;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using Magnum.Pipeline.Visitors;
    using NUnit.Framework;

    [TestFixture]
    public class Interceptor_Specs
    {
        private Pipe _pipe;
        private ManualResetEvent _before;
        private ManualResetEvent _after;

        [SetUp]
        public void Setup()
        {
            _pipe = PipeSegment.Input(PipeSegment.End<object>());
            _before = new ManualResetEvent(false);
            _after = new ManualResetEvent(false);
        }

        [Test]
        public void FirstTestName()
        {
            using (var scope = _pipe.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => _before.Set());
                        x.AfterEachMessage(message => _after.Set());
                    });

                new TracePipeVisitor().Trace(_pipe);

                _pipe.Send(new ClaimModified());
            }

            Assert.IsTrue(_before.WaitOne(TimeSpan.Zero), "Before handler was not called");
            Assert.IsTrue(_after.WaitOne(TimeSpan.Zero), "After handler was not called");
        }
    }
}