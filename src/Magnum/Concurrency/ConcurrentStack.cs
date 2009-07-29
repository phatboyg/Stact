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
namespace Magnum.Concurrency
{
    public class ConcurrentStack<T>
        where T : class
    {
        private SingleLinkNode<T> _head;

        public ConcurrentStack()
        {
            _head = new SingleLinkNode<T>();
        }

        public void Push(T item)
        {
            var newNode = new SingleLinkNode<T>();
            newNode.Item = item;
            do
            {
                newNode.Next = _head.Next;
            } while (!SyncMethods.CAS(ref _head.Next, newNode.Next, newNode));
        }

        public bool Pop(out T item)
        {
            SingleLinkNode<T> node;
            do
            {
                node = _head.Next;
                if (node == null)
                {
                    item = default(T);
                    return false;
                }
            } while (!SyncMethods.CAS(ref _head.Next, node, node.Next));
            item = node.Item;
            return true;
        }

        public T Pop()
        {
            T result;
            Pop(out result);
            return result;
        }
    }
}