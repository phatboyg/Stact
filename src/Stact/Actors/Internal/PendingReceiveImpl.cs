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


    public class PendingReceiveImpl<TState, TMessage> :
        PendingReceive
    {
        readonly Action<PendingReceiveImpl<TState, TMessage>> _onComplete;
        readonly SelectiveConsumer<Message<TMessage>> _selectiveConsumer;
        readonly Action _timeoutCallback;
        bool _cancel;
        ScheduledOperation _scheduledAction;

        public PendingReceiveImpl(SelectiveConsumer<Message<TMessage>> selectiveConsumer,
                                  Action timeoutCallback,
                                  Action<PendingReceiveImpl<TState, TMessage>> onComplete)
        {
            _selectiveConsumer = selectiveConsumer;
            _timeoutCallback = timeoutCallback;
            _onComplete = onComplete;
        }

        public PendingReceiveImpl(SelectiveConsumer<Message<TMessage>> selectiveConsumer,
                                  Action<PendingReceiveImpl<TState, TMessage>> onComplete)
            : this(selectiveConsumer, NoTimeoutCallback, onComplete)
        {
        }

        public void Cancel()
        {
            _cancel = true;

            _onComplete(this);
        }


        public void ScheduleTimeout(Func<PendingReceiveImpl<TState, TMessage>, ScheduledOperation> scheduleAction)
        {
            _scheduledAction = scheduleAction(this);
        }

        static void NoTimeoutCallback()
        {
        }

        public Consumer<Message<TMessage>> Accept(Message<TMessage> message)
        {
            if (_cancel)
                return null;

            Consumer<Message<TMessage>> consumer = _selectiveConsumer(message);
            if (consumer == null)
                return null;

            return m =>
                {
                    CancelTimeout();

                    consumer(m);

                    _onComplete(this);
                };
        }

        public void Timeout()
        {
            if (_cancel)
                return;

            _timeoutCallback();

            _onComplete(this);
        }

        void CancelTimeout()
        {
            if (_scheduledAction != null)
                _scheduledAction.Cancel();
        }
    }
}