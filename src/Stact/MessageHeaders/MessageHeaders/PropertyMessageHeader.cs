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


    public class PropertyMessageHeader<T> :
        MessageHeader,
        SetMessageHeader
    {
        public PropertyMessageHeader()
        {
            Headers = new EmptyHeaders();
        }

        public Uri BodyType
        {
            get { return MessageUrn<T>.Urn; }
        }

        public string MessageId { get; private set; }
        public string CorrelationId { get; private set; }
        public string RequestId { get; private set; }
        public Uri SourceAddress { get; private set; }
        public Uri DestinationAddress { get; private set; }
        public Uri ResponseAddress { get; private set; }
        public Uri FaultAddress { get; private set; }
        public Headers Headers { get; private set; }

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