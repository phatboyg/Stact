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
        protected Pipe Input;
        protected ManualResetEvent BeforeCalled;
        protected ManualResetEvent AfterCalled;

        [SetUp]
        public void Setup()
        {
            Input = PipeSegment.Input(PipeSegment.End<object>());
            BeforeCalled = new ManualResetEvent(false);
            AfterCalled = new ManualResetEvent(false);

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
            using (var scope = Input.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => BeforeCalled.Set());
                        x.AfterEachMessage(message => AfterCalled.Set());
                    });

                Input.Send(new ClaimModified());
            }
        }

        [Test]
        public void Should_call_the_before_method()
        {
            Assert.IsTrue(BeforeCalled.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_call_the_after_method()
        {
            Assert.IsTrue(AfterCalled.WaitOne(TimeSpan.Zero));
        }
    }

    [TestFixture]
    public class Removing_an_interceptor :
        Given_an_empty_pipe
    {
        protected override void EstablishContext()
        {
            using (var scope = Input.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => BeforeCalled.Set());
                        x.AfterEachMessage(message => AfterCalled.Set());
                    });
            }

            Input.Send(new ClaimModified());
        }

        [Test]
        public void Should_not_call_the_before_method()
        {
            Assert.IsFalse(BeforeCalled.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_not_call_the_after_method()
        {
            Assert.IsFalse(AfterCalled.WaitOne(TimeSpan.Zero));
        }
    }

    [TestFixture]
    public class Sending_a_message_after_removing_an_interceptor :
        Given_an_empty_pipe
    {
        protected ManualResetEvent InnerBefore;
        protected ManualResetEvent InnerAfter;

        protected override void EstablishContext()
        {
            InnerBefore = new ManualResetEvent(false);
            InnerAfter = new ManualResetEvent(false);

            using (var scope = Input.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => BeforeCalled.Set());
                        x.AfterEachMessage(message => AfterCalled.Set());
                    });

                using (var innerScope = Input.NewSubscriptionScope())
                {
                    innerScope.Intercept<object>(x =>
                        {
                            x.BeforeEachMessage(() => InnerBefore.Set());
                            x.AfterEachMessage(message => InnerAfter.Set());
                        });
                }

                Input.Send(new ClaimModified());
            }
        }

        [Test]
        public void Should_call_the_remaining_before_method()
        {
            Assert.IsTrue(BeforeCalled.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_call_the_remaining_after_method()
        {
            Assert.IsTrue(AfterCalled.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_not_call_the_removed_before_method()
        {
            Assert.IsFalse(InnerBefore.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_not_call_the_removed_after_method()
        {
            Assert.IsFalse(InnerAfter.WaitOne(TimeSpan.Zero));
        }
    }

    [TestFixture]
    public class Sending_a_message_through_chained_interceptors :
        Given_an_empty_pipe
    {
        protected ManualResetEvent InnerBefore;
        protected ManualResetEvent InnerAfter;

        protected override void EstablishContext()
        {
            InnerBefore = new ManualResetEvent(false);
            InnerAfter = new ManualResetEvent(false);

            using (var scope = Input.NewSubscriptionScope())
            {
                scope.Intercept<object>(x =>
                    {
                        x.BeforeEachMessage(() => BeforeCalled.Set());
                        x.AfterEachMessage(message => AfterCalled.Set());
                    });

                using (var innerScope = Input.NewSubscriptionScope())
                {
                    innerScope.Intercept<object>(x =>
                        {
                            x.BeforeEachMessage(() => InnerBefore.Set());
                            x.AfterEachMessage(message => InnerAfter.Set());
                        });

                    Input.Send(new ClaimModified());
                }
            }
        }

        [Test]
        public void Should_call_the_first_before_method()
        {
            Assert.IsTrue(BeforeCalled.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_call_the_first_after_method()
        {
            Assert.IsTrue(AfterCalled.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_call_the_second_before_method()
        {
            Assert.IsTrue(InnerBefore.WaitOne(TimeSpan.Zero));
        }

        [Test]
        public void Should_call_the_second_after_method()
        {
            Assert.IsTrue(InnerAfter.WaitOne(TimeSpan.Zero));
        }
    }
}