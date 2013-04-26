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
    using System.Threading;
    using System.Threading.Tasks;
    using Internals.Tasks;


    public class MessageQueueExecutor :
        Executor
    {
        readonly MessageDispatcher _dispatcher;
        readonly MessageQueue _queue;

        public MessageQueueExecutor(MessageQueue queue, MessageDispatcher dispatcher)
        {
            _queue = queue;
            _dispatcher = dispatcher;
        }

        Task Executor.Execute(CancellationToken cancellationToken)
        {
            IList<Message> messages = null;
            int index = 0;
            try
            {
                messages = _queue.DequeueAll();
                if (messages.Count == 0)
                    return TaskUtil.Completed();

                for (; index < messages.Count; index++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        RequeueUndispatchedMessages(messages, index);
                        return TaskUtil.Canceled();
                    }

                    messages[index].Dispatch(_dispatcher);
                }

                return TaskUtil.Completed();
            }
            catch (Exception ex)
            {
                RequeueUndispatchedMessages(messages, index + 1);

                return TaskUtil.Faulted(ex);
            }
        }

        void RequeueUndispatchedMessages(IList<Message> messages, int index)
        {
            if (messages == null)
                return;

            if (index < messages.Count)
                _queue.PushFront(messages, index, messages.Count - index);
        }
    }
}