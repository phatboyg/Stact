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


    /// <summary>
    ///   A decorator for sent requests that enables method chaining
    /// </summary>
    /// <typeparam name = "TRequest"></typeparam>
    public class SentRequestImpl<TRequest> :
        SentRequest<TRequest>
    {
        readonly UntypedActor _inbox;
        readonly Message<TRequest> _request;

        public SentRequestImpl(Message<TRequest> request, UntypedActor inbox)
        {
            _request = request;
            _inbox = inbox;
        }

        public Uri BodyType
        {
            get { return _request.BodyType; }
        }

        public string MessageId
        {
            get { return _request.MessageId; }
        }

        public string CorrelationId
        {
            get { return _request.CorrelationId; }
        }

        public string RequestId
        {
            get { return _request.RequestId; }
        }

        public Uri SourceAddress
        {
            get { return _request.SourceAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _request.DestinationAddress; }
        }

        public Uri ResponseAddress
        {
            get { return _request.ResponseAddress; }
        }

        public Uri FaultAddress
        {
            get { return _request.FaultAddress; }
        }

        public Headers Headers
        {
            get { return _request.Headers; }
        }

        public ActorRef Sender
        {
            get { return _request.Sender; }
        }

        public TRequest Body
        {
            get { return _request.Body; }
        }

        public UntypedActor Inbox
        {
            get { return _inbox; }
        }

        public ReceiveHandle ReceiveResponse<T>(SelectiveConsumer<Message<T>> consumer)
        {
            return _inbox.Receive(CreateFilteredConsumer(consumer));
        }

        SelectiveConsumer<Message<T>> CreateFilteredConsumer<T>(SelectiveConsumer<Message<T>> consumer)
        {
            SelectiveConsumer<Message<T>> result = candidate =>
                {
                    if (candidate.RequestId != _request.RequestId)
                        return null;

                    return consumer(candidate);
                };

            return result;
        }
    }
}