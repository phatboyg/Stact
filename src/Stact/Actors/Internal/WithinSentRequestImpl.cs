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
namespace Stact.Internal
{
    using System;


    public class WithinSentRequestImpl<TRequest> :
        WithinSentRequest<TRequest>
    {
        readonly PendingReceiveList _receives;
        readonly SentRequest<TRequest> _request;
        readonly TimeSpan _timeout;
        bool _handled;
        Action _timeoutCallback = DoNothing;

        public WithinSentRequestImpl(SentRequest<TRequest> request, TimeSpan timeout)
        {
            _request = request;
            _timeout = timeout;

            _receives = new PendingReceiveList();
        }

        public WithinSentRequest<TRequest> Receive<T>(SelectiveConsumer<Message<T>> consumer)
        {
            PendingReceive receive = _request.Inbox.Receive<T>(candidate =>
                {
                    Consumer<Message<T>> accepted = consumer(candidate);
                    if (accepted == null)
                        return null;

                    return message =>
                        {
                            accepted(message);

                            _receives.CancelAll();

                            _handled = true;
                        };
                }, _timeout, HandleTimeout);

            _receives.Add(receive);

            return this;
        }

        public WithinSentRequest<TRequest> Receive<T>(Consumer<Message<T>> consumer)
        {
            return Receive<T>(x => consumer);
        }

        public WithinSentRequest<TRequest> Otherwise(Action timeoutCallback)
        {
            _timeoutCallback = timeoutCallback;

            return this;
        }

        void HandleTimeout()
        {
            if (_handled)
                return;

            _timeoutCallback();

            _receives.CancelAll();

            _handled = true;
        }

        static void DoNothing()
        {
        }
    }
}