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
	using Internal;


	public class ChunkHeaderChannel :
		HeaderChannel
	{
		readonly MessageChannel _output;

		public ChunkHeaderChannel(MessageChannel output)
		{
			_output = output;
		}

		public void SendMessage<T>(Message<T> message)
		{
			_output.Send(message.Body, message.Headers.GetDictionary());
		}

		public void SendRequest<T>(Request<T> request)
		{
			_output.Send(request.Body, request.Headers.GetDictionary());
		}

		public void SendResponse<T>(Response<T> response)
		{
			_output.Send(response.Body, response.Headers.GetDictionary());
		}
	}
}