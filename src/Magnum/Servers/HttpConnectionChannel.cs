// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Magnum.Servers
{
	using Channels;
	using Fibers;


	/// <summary>
	/// Handles a connection inside of a channel, allowing handlers to be injected
	/// along the channel network to handle things like authentication and ultimately
	/// routing
	/// </summary>
	public class HttpConnectionChannel :
		Channel<HttpConnectionContext>
	{
		readonly ThreadPoolFiber _fiber;

		public HttpConnectionChannel(ThreadPoolFiber fiber)
		{
			_fiber = fiber;
		}

		public void Send(HttpConnectionContext message)
		{
			_fiber.Add(() => HandleConnection(message));
		}

		void HandleConnection(HttpConnectionContext context)
		{
			_fiber.Add(() =>
				{
					var buffer = new byte[4000];
					int read = context.Request.InputStream.Read(buffer, 0, buffer.Length);


					context.Complete();
				});
		}
	}
}