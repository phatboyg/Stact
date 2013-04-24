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
namespace Stact
{
    using System;
    using System.Linq.Expressions;
    using Internals.Extensions;


    public static class HeaderKey
    {
        public static readonly string BodyType;
        public static readonly string CorrelationId;
        public static readonly string DestinationAddress;
        public static readonly string FaultAddress;
        public static readonly string MessageId;
        public static readonly string RequestId;
        public static readonly string ResponseAddress;
        public static readonly string SourceAddress;

        static HeaderKey()
        {
            BodyType = NameOf(x => x.BodyType);
            CorrelationId = NameOf(x => x.CorrelationId);
            DestinationAddress = NameOf(x => x.DestinationAddress);
            FaultAddress = NameOf(x => x.FaultAddress);
            MessageId = NameOf(x => x.MessageId);
            RequestId = NameOf(x => x.RequestId);
            ResponseAddress = NameOf(x => x.ResponseAddress);
            SourceAddress = NameOf(x => x.SourceAddress);
        }

        static string NameOf<T>(Expression<Func<MessageHeader, T>> expression)
        {
            return expression.GetMemberName();
        }
    }
}