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
    using Magnum.Actors;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using NUnit.Framework;
    using TestFramework;

	[TestFixture]
    public class Connecting_a_filter_to_the_pipeline
    {
        [Test]
        public void Should_pass_derived_types_through_the_filter()
        {
            var received = new Future<bool>();

            Pipe consumer = PipeSegment.Consumer<BaseClass>(x => received.Complete(true));
            Pipe filter = PipeSegment.Filter<object>(consumer);

            filter.Send(new SubClass());

            received.IsAvailable(TimeSpan.Zero).ShouldBeTrue();
        }

        [Test]
        public void Should_not_pass_unwanted_types_through_the_filter()
        {
            var received = new Future<bool>();

            Pipe consumer = PipeSegment.Consumer<SubClass>(x => received.Complete(true));
            Pipe filter = PipeSegment.Filter<object>(consumer);

            filter.Send(new BaseClass());

            received.IsAvailable(TimeSpan.Zero).ShouldBeFalse();
        }

        private class BaseClass
        {
        }

        private class SubClass :
            BaseClass
        {
        }
    }
}