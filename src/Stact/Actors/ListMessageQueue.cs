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
    using System.Collections.Generic;


    public class ListMessageQueue :
        MessageQueue
    {
        static readonly IList<Message> _empty = new List<Message>();
        readonly object _messageLock = new object();
        List<Message> _messages;

        public ListMessageQueue()
        {
            _messages = new List<Message>();
        }

        public void Enqueue<T>(Message<T> message)
        {
            lock (_messageLock)
            {
                _messages.Add(message);
            }
        }

        public IList<Message> DequeueAll()
        {
            lock (_messageLock)
            {
                if (_messages.Count == 0)
                    return _empty;

                List<Message> result = _messages;

                _messages = new List<Message>();

                return result;
            }
        }

        public void PushFront(IList<Message> messages, int offset, int count)
        {
            if (count == 0)
                return;

            lock (_messageLock)
            {
                List<Message> newMessages;
                if (_messages.Count == 0)
                {
                    newMessages = new List<Message>(count);
                    for (int i = offset, copied = 0; copied < count; i++, copied++)
                        newMessages.Add(messages[i]);
                }
                else
                {
                    newMessages = new List<Message>(_messages.Count + count);
                    for (int i = offset, copied = 0; copied < count; i++, copied++)
                        newMessages.Add(messages[i]);
                    newMessages.AddRange(_messages);
                }

                _messages = newMessages;
            }
        }
    }
}