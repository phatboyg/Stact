// Copyright 2007-2008 The Apache Software Foundation.
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
namespace Magnum.Channels
{
	using System;
	using Actions;

	public class InterceptorChannel<T> :
		Channel<T>
	{
		private readonly Channel<T> _output;

		private readonly ActionQueue _queue;

		public InterceptorChannel(ActionQueue queue, Channel<T> output, InterceptorProvider<T> interceptorProvider)
		{
			_queue = queue;
			_output = output;
			InterceptorProvider = interceptorProvider;
		}

		public InterceptorProvider<T> InterceptorProvider { get; private set; }

		public Channel<T> Output
		{
			get { return _output; }
		}

		public void Send(T message)
		{
			_queue.Enqueue(() =>
				{
					Interceptor<T> intercepter = null;
					try
					{
						intercepter = InterceptorProvider.GetInterceptor(message);

						T forward = intercepter.OnMessage(message);

						_output.Send(forward);

						intercepter.OnComplete();
					}
					catch (Exception ex)
					{
						if (intercepter != null)
							intercepter.OnException(ex);
						throw;
					}
				});
		}
	}
}