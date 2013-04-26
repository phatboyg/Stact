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
    /// Supports the dispatch of a message, by type, into a target object without
    /// the need to dynamically type the message using reflection or otherwise
    /// (this eliminates a LOT of duplicate type mapping)
    /// </summary>
    public interface MessageDispatcher
    {
        void DispatchMessage<T>(Message<T> message);
    }


    public interface MessageDispatcher<out TResult>
    {
        TResult DispatchMessage<T>(Message<T> message);
    }
}