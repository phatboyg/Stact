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
namespace Stact.Remote
{
	using System.Collections.Generic;
	using MessageHeaders;


	public class ChunkHeaderChannel :
		MatchHeaderCallback
	{
		readonly HeaderChannel _output;

		public ChunkHeaderChannel(HeaderChannel output)
		{
			_output = output;
		}

		public void Body<TBody>(TBody body)
		{
			var headers = new Dictionary<string, string>();
			headers[MessageMethod.HeaderKey] = MessageMethod.Send;

			_output.Send(body, headers);
		}

		public void Message<TBody>(Message<TBody> message)
		{
			message.Headers[MessageMethod.HeaderKey] = MessageMethod.Send;

			_output.Send(message.Body, message.Headers.GetDictionary());
		}

		public void Request<TRequest>(Request<TRequest> request)
		{
			request.Headers[MessageMethod.HeaderKey] = MessageMethod.Request;
	
			_output.Send(request.Body, request.Headers.GetDictionary());
		}

		public void Response<TResponse>(Response<TResponse> response)
		{
			response.Headers[MessageMethod.HeaderKey] = MessageMethod.Response;

			_output.Send(response.Body, response.Headers.GetDictionary());
		}
	}
}