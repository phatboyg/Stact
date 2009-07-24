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
namespace Magnum.Specs.Pipeline.Consumers
{
    using Magnum.Actors;
    using Magnum.Pipeline;
    using Messages;

    public class MultipleMessageConsumer :
        IConsume<ClaimModified>,
        IConsume<ClaimCreated>
    {
        public Future<ClaimModified> ClaimModifiedCalled { get; private set; }
        public Future<ClaimCreated> ClaimCreatedCalled { get; private set; }

        public MultipleMessageConsumer()
        {
            ClaimModifiedCalled = new Future<ClaimModified>();
            ClaimCreatedCalled = new Future<ClaimCreated>();
        }

        public void Consume(ClaimModified message)
        {
            ClaimModifiedCalled.Complete(message);
        }

        public void Consume(ClaimCreated message)
        {
            ClaimCreatedCalled.Complete(message);
        }
    }
}