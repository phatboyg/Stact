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
	using Magnum.Extensions;


	public static class HeaderKey
	{
		public static readonly string BodyType;
		public static readonly string CorrelationId;
		public static readonly string DestinationAddress;
		public static readonly string FaultAddress;
		public static readonly string MessageId;
		public static readonly string RequestId;
		public static readonly string ResponseAddress;
		public static readonly string SenderAddress;
		public static readonly string Method = "Method";

		static HeaderKey()
		{
			BodyType = NameOf(x => x.BodyType);
			MessageId = NameOf(x => x.MessageId);
			CorrelationId = NameOf(x => x.CorrelationId);
			RequestId = NameOf(x => x.RequestId);
			SenderAddress = NameOf(x => x.SenderAddress);
			DestinationAddress = NameOf(x => x.DestinationAddress);
			FaultAddress = NameOf(x => x.FaultAddress);
			ResponseAddress = NameOf(x => x.ResponseAddress);
		}

		static string NameOf(Expression<Func<AllHeaders, object>> expression)
		{
			return expression.MemberName();
		}


		interface AllHeaders :
			RequestHeader
		{
		}
	}
}