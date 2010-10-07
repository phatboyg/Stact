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
namespace Stact.Channels
{
	using System.Threading;
	


	/// <summary>
	/// Using the specified SynchronizationContext, messages sent through this channel
	/// will be delivered on the specified user interface thread, to avoid issues when
	/// writing to the UI
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public class SynchronizedChannel :
		UntypedChannel
	{
		readonly Fiber _fiber;
		readonly object _state;
		readonly SynchronizationContext _synchronizationContext;

		public SynchronizedChannel(Fiber fiber, UntypedChannel output, SynchronizationContext synchronizationContext)
			: this(fiber, output, synchronizationContext, 0)
		{
		}

		public SynchronizedChannel(Fiber fiber, UntypedChannel output, SynchronizationContext synchronizationContext,
		                           object state)
		{
			_fiber = fiber;
			Output = output;
			_synchronizationContext = synchronizationContext;
			_state = state;
		}

		public UntypedChannel Output { get; private set; }

		public void Send<T>(T message)
		{
			_fiber.Add(() => _synchronizationContext.Send(x => Output.Send(message), _state));
		}
	}


	/// <summary>
	/// Using the specified SynchronizationContext, messages sent through this channel
	/// will be delivered on the specified user interface thread, to avoid issues when
	/// writing to the UI
	/// </summary>
	/// <typeparam name="T">The channel type</typeparam>
	public class SynchronizedChannel<T> :
		Channel<T>
	{
		readonly Fiber _fiber;
		readonly object _state;
		readonly SynchronizationContext _synchronizationContext;

		public SynchronizedChannel(Fiber fiber, Channel<T> output, SynchronizationContext synchronizationContext)
			: this(fiber, output, synchronizationContext, 0)
		{
		}

		public SynchronizedChannel(Fiber fiber, Channel<T> output, SynchronizationContext synchronizationContext, object state)
		{
			_fiber = fiber;
			Output = output;
			_synchronizationContext = synchronizationContext;
			_state = state;
		}

		public Channel<T> Output { get; private set; }

		public void Send(T message)
		{
			_fiber.Add(() => _synchronizationContext.Send(x => Output.Send(message), _state));
		}
	}
}