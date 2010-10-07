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
	

	/// <summary>
	/// An interceptor channel can be placed in front of any other channel and provides
	/// the ability to intercept sends to the wrapper channel and modify the message that
	/// is sent. An additional method is also called once the message has been processed by
	/// the wrapped channel
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public class InterceptorChannel<T> :
		Channel<T>
	{
		private readonly Fiber _fiber;
		private readonly Channel<T> _output;

		public InterceptorChannel(Fiber fiber, Channel<T> output, InterceptorFactory<T> interceptorFactory)
		{
			_fiber = fiber;
			_output = output;
			InterceptorFactory = interceptorFactory;
		}

		public InterceptorFactory<T> InterceptorFactory { get; private set; }

		public Channel<T> Output
		{
			get { return _output; }
		}

		public void Send(T message)
		{
			_fiber.Add(() => InterceptMessage(message));
		}

		private void InterceptMessage(T message)
		{
			Interceptor<T> intercepter = null;
			try
			{
				intercepter = InterceptorFactory.GetInterceptor(message);

				T forward = intercepter.OnSend(message);

				// NOTE that if the output channel is not a synchronous channel, the intercepter will not
				// control the context of that channel and the processing of the message
				_output.Send(forward);

				intercepter.OnComplete();
			}
			catch (Exception ex)
			{
				if (intercepter != null)
					intercepter.OnException(ex);
				throw;
			}
		}
	}
}