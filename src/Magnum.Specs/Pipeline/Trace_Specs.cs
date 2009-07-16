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
    using System.Diagnostics;
    using Magnum.Pipeline;
    using Magnum.Pipeline.Segments;
    using Magnum.Pipeline.Visitors;
    using NUnit.Framework;

    [TestFixture]
    public class The_TracePipeInspector
    {
        [SetUp]
        public void Setup()
        {
            Trace.WriteLine("");
        }

        [Test]
        public void Should_display_an_EndSegment()
        {
            Pipe end = PipeSegment.End<object>();

            new TracePipeVisitor().Trace(end);
        }

        [Test]
        public void Should_display_an_InputSegment()
        {
            var end = PipeSegment.End<object>();
            Pipe input = PipeSegment.Input(end);

            new TracePipeVisitor().Trace(input);
        }

        [Test]
        public void Should_display_a_FilterSegment()
        {
            var end = PipeSegment.End<ClaimModified>();
            Pipe filter = PipeSegment.Filter<object>(end);

            new TracePipeVisitor().Trace(filter);
        }

        [Test]
        public void Should_display_an_empty_RecipientListSegment()
        {
            Pipe recipientList = PipeSegment.RecipientList<ClaimModified>(new Pipe[] {});
            new TracePipeVisitor().Trace(recipientList);
        }

        [Test]
        public void Should_display_a_RecipientListSegment_with_one_child_segment()
        {
            var end = PipeSegment.End<ClaimModified>();
            Pipe recipientList = PipeSegment.RecipientList<ClaimModified>(new Pipe[] {end});
            new TracePipeVisitor().Trace(recipientList);
        }

        [Test]
        public void Should_display_a_MessageConsumerSegment()
        {
            Pipe consumer = PipeSegment.Consumer<ClaimModified>(x => { });

            new TracePipeVisitor().Trace(consumer);
        }

        [Test]
        public void Should_display_a_complex_segment_chain()
        {
            var consumer = PipeSegment.Consumer<ClaimModified>(x => { });
            var end = PipeSegment.End<ClaimModified>();
            var recipientList = PipeSegment.RecipientList<ClaimModified>(new Pipe[] {consumer, end});
            var filter = PipeSegment.Filter<object>(recipientList);
            Pipe input = PipeSegment.Input(filter);

            new TracePipeVisitor().Trace(input);
        }
    }
}