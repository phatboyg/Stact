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
	public class ResponseImpl<TResponse> :
		MessageImpl<TResponse>,
		Response<TResponse>
	{
		public ResponseImpl(TResponse message)
			: base(message)
		{
		}
	}


	public class ResponseImpl<TRequest, TResponse> :
		MessageImpl<TResponse>,
		Response<TRequest, TResponse>
	{
		public ResponseImpl(TRequest request, TResponse message)
			: base(message)
		{
			Request = request;
		}

		public TRequest Request { get; private set; }
	}
}