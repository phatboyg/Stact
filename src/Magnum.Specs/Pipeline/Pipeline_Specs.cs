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
    using FunctionalBits.Pipeline;
    using NUnit.Framework;

    [TestFixture]
    public class Pipeline_Specs
    {
        [Test]
        public void First()
        {
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
        PipelineVisitor
    {
    }

    public static class PipelineExtensionMethods
    {
        public static IPipeline Subscribe<T>(this IPipeline pipeline, MessageConsumer<T> consumer, out Unsubscriber unsubscriber)
        {
            unsubscriber = () => { };

//            new RecipientListVisitor<T>(recipientList => recipientList.Add(consumer));


            return pipeline;
        }
    }
}