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
	using Magnum;


	public class RequestImpl<T> :
		MessageImpl<T>,
		Request<T>
	{
		public RequestImpl(UntypedChannel responseChannel, T message)
			: this(responseChannel, message, CombGuid.Generate().ToString("N"))
		{
		}

		public RequestImpl(UntypedChannel responseChannel, T message, string requestId)
			: base(message)
		{
			ResponseChannel = responseChannel;
			RequestId = requestId;
		}

		public UntypedChannel ResponseChannel { get; private set; }

		public string RequestId
		{
			get { return Headers[HeaderKey.RequestId]; }
			set { Headers[HeaderKey.RequestId] = value; }
		}

		public Uri ResponseAddress
		{
			get { return Headers.GetUri(HeaderKey.ResponseAddress); }
			set { Headers.SetUri(HeaderKey.ResponseAddress, value); }
		}

	}
}