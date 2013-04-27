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
namespace Stact.MessageHeaders
{
    using System;
    using System.Threading;
    using Actors;


    public class MessageContext<T> :
        Message<T>,
        SetMessageHeader
    {
        readonly PropertyMessageHeader<T> _header;
        readonly Lazy<ActorRef> _sender;

        public MessageContext(T message)
            : this(message, () => new NullActorReference())
        {
        }

        public MessageContext(T message, ActorRef sender)
            : this(message, () => sender)
        {
        }

        public MessageContext(T message, Func<ActorRef> sender)
        {
            Body = message;

            _header = new PropertyMessageHeader<T>();
            _sender = new Lazy<ActorRef>(sender, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Uri BodyType
        {
            get { return _header.BodyType; }
        }

        public string MessageId
        {
            get { return _header.MessageId; }
        }

        public string CorrelationId
        {
            get { return _header.CorrelationId; }
        }

        public string RequestId
        {
            get { return _header.RequestId; }
        }

        public Uri SourceAddress
        {
            get { return _header.SourceAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _header.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _header.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _header.FaultAddress; }
        }

        public Headers Headers
        {
            get { return _header.Headers; }
        }

        public T Body { get; private set; }

        public ActorRef Sender
        {
            get { return _sender != null ? _sender.Value : null; }
        }

        public void Dispatch(MessageDispatcher dispatcher)
        {
            dispatcher.DispatchMessage(this);
        }

        public TResult Dispatch<TResult>(MessageDispatcher<TResult> dispatcher)
        {
            return dispatcher.DispatchMessage(this);
        }

        public SetMessageHeader SetMessageId(string messageId)
        {
            return _header.SetMessageId(messageId);
        }

        public SetMessageHeader SetRequestId(string requestId)
        {
            return _header.SetRequestId(requestId);
        }

        public SetMessageHeader SetCorrelationId(string correlationId)
        {
            return _header.SetCorrelationId(correlationId);
        }

        public SetMessageHeader SetSourceAddress(Uri sourceAddress)
        {
            return _header.SetSourceAddress(sourceAddress);
        }

        public SetMessageHeader SetDestinationAddress(Uri destinationAddress)
        {
            return _header.SetDestinationAddress(destinationAddress);
        }

        public SetMessageHeader SetResponseAddress(Uri responseAddress)
        {
            return _header.SetResponseAddress(responseAddress);
        }

        public SetMessageHeader SetFaultAddress(Uri faultAddress)
        {
            return _header.SetFaultAddress(faultAddress);
        }
    }
}