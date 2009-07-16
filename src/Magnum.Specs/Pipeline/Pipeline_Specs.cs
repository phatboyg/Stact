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
    using Magnum.Pipeline.Segments;
    using Magnum.Pipeline.Visitors;
    using NUnit.Framework;

    [TestFixture]
    public class Subscribing_to_the_pipe
    {
        [Test]
        public void Should_result_in_an_expression_being_called()
        {
            ManualResetEvent called = new ManualResetEvent(false);

            var consumer = PipeSegment.Consumer<ClaimModified>(message => { called.Set(); });

            var recipients = new[] {consumer};

            var recipientList = PipeSegment.RecipientList<ClaimModified>(recipients);

            new TracePipeVisitor().Trace(recipientList);

            recipientList.Send(new ClaimModified());

            Assert.IsTrue(called.WaitOne(TimeSpan.Zero, false));
        }
    }

    public class ClaimModified :
        IDomainEvent
    {
        public string Text { get; set; }
    }

    public interface IDomainEvent
    {
    }

    public class RecipientListVisitor<T> :
        AbstractPipeVisitor
    {
    }
}