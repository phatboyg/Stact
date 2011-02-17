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


	public class MessageImpl<T> :
		Message<T>
	{
		readonly HeadersImpl _headers;

		public MessageImpl(T message)
		{
			Body = message;
			_headers = new HeadersImpl();

			_headers[HeaderKey.BodyType] = typeof(T).ToMessageUrn().ToString();
		}

		public T Body { get; private set; }

		public Uri BodyType
		{
			get { return _headers.GetUri(HeaderKey.BodyType); }
		}

		public string MessageId
		{
			get { return _headers[HeaderKey.MessageId]; }
			set { _headers[HeaderKey.MessageId] = value; }
		}

		public string CorrelationId
		{
			get { return _headers[HeaderKey.CorrelationId]; }
			set { _headers[HeaderKey.CorrelationId] = value; }
		}

		public Uri SenderAddress
		{
			get { return _headers.GetUri(HeaderKey.SenderAddress); }
			set { _headers.SetUri(HeaderKey.SenderAddress, value); }
		}

		public Uri DestinationAddress
		{
			get { return _headers.GetUri(HeaderKey.DestinationAddress); }
			set { _headers.SetUri(HeaderKey.DestinationAddress, value); }
		}

		public Uri FaultAddress
		{
			get { return _headers.GetUri(HeaderKey.FaultAddress); }
			set { _headers.SetUri(HeaderKey.FaultAddress, value); }
		}

		public Headers Headers
		{
			get { return _headers; }
		}
	}
}