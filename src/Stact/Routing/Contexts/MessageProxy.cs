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


    public class MessageProxy<TInput, TOutput> :
        Message<TOutput>
        where TInput : TOutput
    {
        readonly Message<TInput> _message;

        public MessageProxy(Message<TInput> message)
        {
            _message = message;
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

        public TOutput Body
        {
            get { return _message.Body; }
        }
    }
}