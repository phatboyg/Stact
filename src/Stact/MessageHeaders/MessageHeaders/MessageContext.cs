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
namespace Stact.MessageHeaders
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Actors;


    public class MessageContext<T> :
        Message<T>,
        SetMessageHeader
    {
        readonly DictionaryHeaders _headers;

        readonly Lazy<ActorRef> _sender;

        public MessageContext(T message)
        {
            Body = message;
            _headers = new DictionaryHeaders();
            _sender = new Lazy<ActorRef>(() => new NullActorReference(), LazyThreadSafetyMode.PublicationOnly);

            _headers[HeaderKey.BodyType] = MessageUrn<T>.UrnString;
        }

        public MessageContext(T message, Func<ActorRef> sender)
            : this(message)
        {
            _sender = new Lazy<ActorRef>(sender, true);
        }

        public MessageContext(T message, IDictionary<string, string> headers)
        {
            Body = message;
            _headers = new DictionaryHeaders(headers);
            _sender = new Lazy<ActorRef>(() => new NullActorReference(), LazyThreadSafetyMode.PublicationOnly);

            _headers[HeaderKey.BodyType] = MessageUrn<T>.UrnString;
        }

        public T Body { get; private set; }

        public ActorRef Sender
        {
            get { return _sender != null ? _sender.Value : null; }
        }

        public Uri BodyType
        {
            get { return _headers.GetUri(HeaderKey.BodyType); }
        }

        public string MessageId
        {
            get { return _headers[HeaderKey.MessageId]; }
            private set { _headers[HeaderKey.MessageId] = value; }
        }

        public string CorrelationId
        {
            get { return _headers[HeaderKey.CorrelationId]; }
            private set { _headers[HeaderKey.CorrelationId] = value; }
        }

        public string RequestId
        {
            get { return _headers[HeaderKey.RequestId]; }
            private set { _headers[HeaderKey.RequestId] = value; }
        }

        public Uri SourceAddress
        {
            get { return _headers.GetUri(HeaderKey.SourceAddress); }
            private set { _headers.SetUri(HeaderKey.SourceAddress, value); }
        }

        public Uri DestinationAddress
        {
            get { return _headers.GetUri(HeaderKey.DestinationAddress); }
            private set { _headers.SetUri(HeaderKey.DestinationAddress, value); }
        }

        public Uri ResponseAddress
        {
            get { return _headers.GetUri(HeaderKey.ResponseAddress); }
            private set { _headers.SetUri(HeaderKey.ResponseAddress, value); }
        }

        public Uri FaultAddress
        {
            get { return _headers.GetUri(HeaderKey.FaultAddress); }
            private set { _headers.SetUri(HeaderKey.FaultAddress, value); }
        }

        public Headers Headers
        {
            get { return _headers; }
        }

        public SetMessageHeader SetMessageId(string messageId)
        {
            MessageId = messageId;
            return this;
        }

        public SetMessageHeader SetRequestId(string requestId)
        {
            RequestId = requestId;
            return this;
        }

        public SetMessageHeader SetCorrelationId(string correlationId)
        {
            CorrelationId = correlationId;
            return this;
        }

        public SetMessageHeader SetSourceAddress(Uri sourceAddress)
        {
            SourceAddress = sourceAddress;
            return this;
        }

        public SetMessageHeader SetDestinationAddress(Uri destinationAddress)
        {
            DestinationAddress = destinationAddress;
            return this;
        }

        public SetMessageHeader SetResponseAddress(Uri responseAddress)
        {
            ResponseAddress = responseAddress;
            return this;
        }

        public SetMessageHeader SetFaultAddress(Uri faultAddress)
        {
            FaultAddress = faultAddress;
            return this;
        }
    }
}