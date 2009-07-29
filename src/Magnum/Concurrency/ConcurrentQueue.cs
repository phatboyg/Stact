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
    using System.Threading;

    /// <summary>
    /// A concurrent queue with no sychronization locks, locking is performed using
    /// atomic operations to avoid blocking
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConcurrentQueue<T>
        where T : class
    {
        private SingleLinkNode<T> _head;
        private SingleLinkNode<T> _tail;

        public ConcurrentQueue()
        {
            _head = new SingleLinkNode<T>();
            _tail = _head;
        }

        public void Enqueue(T item)
        {
            SingleLinkNode<T> oldTail = null;
            SingleLinkNode<T> oldTailNext;

            var newNode = new SingleLinkNode<T> {Item = item};

            bool newNodeWasAdded = false;
            while (!newNodeWasAdded)
            {
                oldTail = _tail;
                oldTailNext = oldTail.Next;

                if (_tail == oldTail)
                {
                    if (oldTailNext == null)
                        newNodeWasAdded = SyncMethods.CAS(ref _tail.Next, null, newNode);
                    else
                        SyncMethods.CAS(ref _tail, oldTail, oldTailNext);
                }
            }

            SyncMethods.CAS(ref _tail, oldTail, newNode);
        }

        public bool Dequeue(out T item)
        {
            item = default(T);
            SingleLinkNode<T> oldHead = null;

            bool haveAdvancedHead = false;
            while (!haveAdvancedHead)
            {
                oldHead = _head;
                SingleLinkNode<T> oldTail = _tail;
                SingleLinkNode<T> oldHeadNext = oldHead.Next;

                if (oldHead == _head)
                {
                    if (oldHead == oldTail)
                    {
                        if (oldHeadNext == null)
                        {
                            return false;
                        }

                        SyncMethods.CAS(ref _tail, oldTail, oldHeadNext);
                    }

                    else
                    {
                        item = oldHeadNext.Item;
                        haveAdvancedHead = SyncMethods.CAS(ref _head, oldHead, oldHeadNext);
                    }
                }
            }
            return true;
        }

        public T Dequeue()
        {
            T result;
            Dequeue(out result);
            return result;
        }
    }

    public static class SyncMethods
    {
        public static bool CAS<T>(ref T location, T comparand, T newValue) where T : class
        {
            return comparand == Interlocked.CompareExchange(ref location, newValue, comparand);
        }

        public static bool CompareAndExchange<T>(this ref T location, T comparand, T newValue)
            where T : class
        {
            return comparand == Interlocked.CompareExchange(ref location, newValue, comparand);
        }
    }
}