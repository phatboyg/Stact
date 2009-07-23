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
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using Magnum.Pipeline.Visitors;
    using NUnit.Framework;

    [TestFixture]
    public class When_adding_consumers_to_an_empty_typed_pipeline
    {
        private Pipe _pipe;
        private ISubscriptionScope _scope;

        [SetUp]
        public void Setup()
        {
            _pipe = PipeSegment.Input(PipeSegment.End<ClaimModified>());
            _scope = _pipe.NewSubscriptionScope();
            _scope.Subscribe<ClaimModified>(x => { });
        }

        [TearDown]
        public void Teardown()
        {
            _scope.Dispose();
        }

        [Test]
        public void A_single_consumer_should_not_create_a_recipient_list()
        {
            new TracePipeVisitor().Trace(_pipe);

            _pipe.Send(new ClaimCreated());
        }

        [Test]
        public void Multiple_consumers_should_create_a_recipient_list()
        {
            _scope.Subscribe<ClaimModified>(x => { });

            new TracePipeVisitor().Trace(_pipe);

            _pipe.Send(new ClaimCreated());
        }
    }
}