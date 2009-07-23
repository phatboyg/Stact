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
    using NUnit.Framework;

    [TestFixture]
    public class Given_an_empty_pipe
    {
        protected Pipe _pipe;
        protected ManualResetEvent _before;
        protected ManualResetEvent _after;

        [SetUp]
        public void Setup()
        {
            _pipe = PipeSegment.Input(PipeSegment.End<object>());
            _before = new ManualResetEvent(false);
            _after = new ManualResetEvent(false);

            EstablishContext();
        }

        protected virtual void EstablishContext()
        {
        }
    }

 

    [TestFixture]
    public class Adding_an_interceptor_with_before_and_after_methods :
        Given_an_empty_pipe
    {
        protected override void EstablishContext()
        {
            using (var scope = _pipe.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => _before.Set());
                        x.AfterEachMessage(message => _after.Set());
                    });

                _pipe.Send(new ClaimModified());
            }
        }

        [Test]
        public void Should_call_the_before_method()
        {
            Assert.IsTrue(_before.WaitOne(TimeSpan.Zero), "Before handler was not called");
        }

        [Test]
        public void Should_call_the_after_method()
        {
            Assert.IsTrue(_after.WaitOne(TimeSpan.Zero), "After handler was not called");
        }
    }

    [TestFixture]
    public class Removing_an_interceptor :
        Given_an_empty_pipe
    {
        protected override void EstablishContext()
        {
            using (var scope = _pipe.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => _before.Set());
                        x.AfterEachMessage(message => _after.Set());
                    });
            }

            _pipe.Send(new ClaimModified());
        }

        [Test]
        public void Should_not_call_the_before_method()
        {
            Assert.IsFalse(_before.WaitOne(TimeSpan.Zero), "Before handler was not called");
        }

        [Test]
        public void Should_not_call_the_after_method()
        {
            Assert.IsFalse(_after.WaitOne(TimeSpan.Zero), "After handler was not called");
        }
    }

    [TestFixture]
    public class Removing_an_interceptor_with_nested_scope :
        Given_an_empty_pipe
    {
        protected ManualResetEvent _innerBefore;
        protected ManualResetEvent _innerAfter;

        protected override void EstablishContext()
        {
            _innerBefore = new ManualResetEvent(false);
            _innerAfter = new ManualResetEvent(false);

            using (var scope = _pipe.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => _before.Set());
                        x.AfterEachMessage(message => _after.Set());
                    });

                using (var innerScope = _pipe.NewSubscriptionScope())
                {
                    innerScope.Intercept<object>(x =>
                        {
                            x.BeforeEachMessage(() => _innerBefore.Set());
                            x.AfterEachMessage(message => _innerAfter.Set());
                        });
                }

                _pipe.Send(new ClaimModified());
            }
        }

        [Test]
        public void Should_not_call_the_before_method()
        {
            Assert.IsTrue(_before.WaitOne(TimeSpan.Zero), "Before handler was not called");
            Assert.IsFalse(_innerBefore.WaitOne(TimeSpan.Zero), "Before handler was not called");
        }

        [Test]
        public void Should_not_call_the_after_method()
        {
            Assert.IsTrue(_after.WaitOne(TimeSpan.Zero), "After handler was not called");
            Assert.IsFalse(_innerAfter.WaitOne(TimeSpan.Zero), "After handler was not called");
        }
    }
}