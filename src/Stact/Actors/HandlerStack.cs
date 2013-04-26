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
    using System;
    using System.Collections.Generic;


    public class HandlerStack<T>
        where T : class
    {
        readonly Stack<T> _stack;

        public HandlerStack(T first)
        {
            _stack = new Stack<T>();
            _stack.Push(first);
        }

        public void Push(T handler)
        {
            _stack.Push(handler);
        }

        public void Pop(T handler)
        {
            if (_stack.Contains(handler))
            {
                while (_stack.Pop() != handler)
                    ;
            }
        }

        public void Enumerate(Action<IEnumerator<T>> callback)
        {
            using (Stack<T>.Enumerator enumerator = _stack.GetEnumerator())
            {
                // need to capture this or it totally blows up due to closure
                Stack<T>.Enumerator handlerEnumerator = enumerator;

                callback(handlerEnumerator);
            }
        }
    }
}