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
namespace FunctionalBits.Pipeline
{
    using MbUnit.Framework;

    [TestFixture]
    public class Trace_Specs
    {
        [Test]
        public void The_pipeline_trace_should_display_the_entire_structure_of_the_pipeline()
        {
            IPipeline pipeline = PipelineNode.End<object>();

            new TracePipelineVisitor().Trace(pipeline);
        }

        [Test]
        public void The_pipeline_trace_should_display_the_nature_structure_of_the_pipeline()
        {
            var end = PipelineNode.End<object>();

            var input = PipelineNode.Input(end);

            new TracePipelineVisitor().Trace(input);
        }

        [Test]
        public void A_typed_pipeline_node_should_change_the_type_chain()
        {
            var end = PipelineNode.End<ClaimModified>();
            var filter = PipelineNode.Filter<object>(end);
            var input = PipelineNode.Input(filter);

            new TracePipelineVisitor().Trace(input);
        }

        [Test]
        public void A_message_recipient_list_should_be_viewable()
        {
            var end = PipelineNode.End<ClaimModified>();
            var recipientList = PipelineNode.RecipientList<ClaimModified>(new IPipeline[] {end});
            var filter = PipelineNode.Filter<object>(recipientList);
            var input = PipelineNode.Input(filter);

            new TracePipelineVisitor().Trace(input);
        }
    }
}