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
namespace Stact.Routing.Contexts
{
    using System;


    public class RequestMessageProxy<TInput> :
        Request<TInput>
    {
        static readonly Uri _nullAddress = new Uri("null://localhost/null");
        static readonly UntypedChannel _shunt = new ShuntChannel();
        readonly Message<TInput> _message;
        string _requestId;

        public RequestMessageProxy(Message<TInput> message)
        {
            _message = message;

            _requestId = Guid.NewGuid().ToString("N");
        }

        public Uri BodyType
        {
            get { return _message.BodyType; }
        }

        public string MessageId
        {
            get { return _message.MessageId; }
        }

        public string CorrelationId
        {
            get { return _message.CorrelationId; }
        }

        public Uri SenderAddress
        {
            get { return _message.SenderAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _message.DestinationAddress; }
        }

        public Uri FaultAddress
        {
            get { return _message.FaultAddress; }
        }

        public Headers Headers
        {
            get { return _message.Headers; }
        }

        public TInput Body
        {
            get { return _message.Body; }
        }

        public string RequestId
        {
            get { return _requestId; }
        }

        public Uri ResponseAddress
        {
            get { return _nullAddress; }
        }

        public UntypedChannel ResponseChannel
        {
            get { return _shunt; }
        }
    }
}