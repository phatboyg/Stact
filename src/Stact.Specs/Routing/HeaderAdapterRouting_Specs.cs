// Copyright 2010 Chris Patterson
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
namespace Stact.Specs
{
    using Headers;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using NUnit.Framework;
    using Routing;


    [Scenario]
    public class When_sending_a_raw_message
    {
        [Then]
        public void Should_upconvert_to_a_message_of_t()
        {
            var received = new Future<Message<Simple>>();

            var engine = new DynamicRoutingEngine(new PoolFiber());
            engine.Configure(x => x.Receive<Message<Simple>>(received.Complete));

            engine.Send(new SimpleImpl());

            received.WaitUntilCompleted(6.Seconds()).ShouldBeTrue();
            received.Value.ShouldNotBeNull();
        }

        [Then]
        [Ignore("We don't upgrade to request since we don't have enough data")]
        public void Should_upconvert_to_a_request_of_t()
        {
            var received = new Future<Request<Simple>>();

            var engine = new DynamicRoutingEngine(new PoolFiber());
            engine.Configure(x => x.Receive<Request<Simple>>(received.Complete));

            engine.Send(new SimpleImpl());

            received.WaitUntilCompleted(6.Seconds()).ShouldBeTrue();
            received.Value.ShouldNotBeNull();
        }
    }
}