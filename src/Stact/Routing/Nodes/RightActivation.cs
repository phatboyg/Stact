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
namespace Stact.Routing.Nodes
{
    using System;


    /// <summary>
    /// A RightActivation moves a message along the right side of the graph, which is
    /// referred to as the beta network or join network.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface RightActivation<T>
    {
        /// <summary>
        /// Performs a join operation between the two messages, performing the callback for
        /// every activation on the right node until the callback returns false
        /// </summary>
        /// <param name="callback"></param>
        void RightActivate(Func<RoutingContext<T>, bool> callback);

        /// <summary>
        /// Performs a match operation between two activations, 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback"></param>
        void RightActivate(RoutingContext<T> context, Action<RoutingContext<T>> callback);
    }
}