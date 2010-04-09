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
	using System.Threading;
	using Actions;

	/// <summary>
	/// Using the specified SynchronizationContext, messages sent through this channel
	/// will be delivered on the specified user interface thread, to avoid issues when
	/// writing to the UI
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public class SynchronizedChannel<T> :
		Channel<T>
	{
		private readonly Channel<T> _output;
		private readonly ActionQueue _queue;
		private readonly object _state;
		private readonly SynchronizationContext _synchronizationContext;

		public SynchronizedChannel(ActionQueue queue, Channel<T> output, SynchronizationContext synchronizationContext)
		{
			_queue = queue;
			_output = output;
			_state = 0;
			_synchronizationContext = synchronizationContext;
		}

		public void Send(T message)
		{
			_queue.Enqueue(() => _synchronizationContext.Send(x => _output.Send(message), _state));
		}
	}
}