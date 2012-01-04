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
namespace Stact
{
    /// <summary>
    /// The syntax for an exit handler. When defined in a behavior, this will intercept
    /// an exit message sent to an actor, allowing the behavior to customize the exit
    /// handling
    /// </summary>
    /// <param name="message">The exit message</param>
    /// <param name="next"></param>
    public delegate void ActorExitHandler(Message<Exit> message, NextExitHandler next);
}