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
namespace Stact.Behaviors
{
    /// <summary>
    /// An abstract reference to a behavior that has been applied to an actor.
    /// </summary>
    public interface BehaviorHandle
    {
        /// <summary>
        /// Removes the behavior from the actor, which includes canceling any
        /// pending receive calls from the inbox.
        /// </summary>
        void Remove();
    }
}