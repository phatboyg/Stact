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


    public class RequestProxy<TInput, TOutput> :
        Request<TOutput>
        where TInput : TOutput
    {
        readonly Request<TInput> _request;

        public RequestProxy(Request<TInput> request)
        {
            _request = request;
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

        public Uri SenderAddress
        {
            get { return _request.SenderAddress; }
        }

        public Uri DestinationAddress
        {
            get { return _request.DestinationAddress; }
        }

        public Uri FaultAddress
        {
            get { return _request.FaultAddress; }
        }

        public Headers Headers
        {
            get { return _request.Headers; }
        }

        public TOutput Body
        {
            get { return _request.Body; }
        }

        public string RequestId
        {
            get { return _request.RequestId; }
        }

        public Uri ResponseAddress
        {
            get { return _request.ResponseAddress; }
        }

        public UntypedChannel ResponseChannel
        {
            get { return _request.ResponseChannel; }
        }
    }
}