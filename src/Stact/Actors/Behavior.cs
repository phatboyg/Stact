// Copyright 2010-2013 Chris Patterson
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
    /// An actor behavior that is not dependent upon the actor state can be defined
    /// using this interface. An example of a behavior not using the state is something
    /// like an exception handling behavior or a supervisor behavior. A default constructor
    /// or a constructor that accepts an ActorInbox is expected.
    /// </summary>
    public interface Behavior
    {
    }


    /// <summary>
    /// A behavior that accesses the state of the actor for whatever purpose. A constructor
    /// that accepts the Actor<typeparamref name="TState"/> should be found on the behavior
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public interface Behavior<in TState> :
        Behavior
    {
    }
}