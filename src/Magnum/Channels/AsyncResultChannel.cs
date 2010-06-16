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
	using System.Threading;

	/// <summary>
	/// Wraps a channel in an IAsyncResult compatible wrapper to support asynchronous usage with
	/// frameworks that support asynchronous callbacks
	/// </summary>
	/// <typeparam name="T">The channel type supported</typeparam>
	public class AsyncResultChannel :
		UntypedChannel,
		IAsyncResult
	{
		private readonly AsyncCallback _callback;

		private readonly object _state;
		private volatile bool _completed;

		public AsyncResultChannel(UntypedChannel output, AsyncCallback callback, object state)
		{
			Guard.AgainstNull(output, "output");
			Guard.AgainstNull(callback, "callback");

			Output = output;
			_callback = callback;
			_state = state;
		}

		public UntypedChannel Output { get; private set; }

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { throw new NotSupportedException("Wait handles are not supported by the channel framework"); }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public void Send<T>(T message)
		{
			Output.Send(message);

			Complete();
		}

		private void Complete()
		{
			_completed = true;

			_callback(this);
		}
	}

	/// <summary>
	/// Wraps a channel in an IAsyncResult compatible wrapper to support asynchronous usage with
	/// frameworks that support asynchronous callbacks
	/// </summary>
	/// <typeparam name="T">The channel type supported</typeparam>
	public class AsyncResultChannel<T> :
		Channel<T>,
		IAsyncResult
	{
		private readonly AsyncCallback _callback;

		private readonly object _state;
		private volatile bool _completed;

		public AsyncResultChannel(Channel<T> output, AsyncCallback callback, object state)
		{
			Guard.AgainstNull(output, "output");
			Guard.AgainstNull(callback, "callback");

			Output = output;
			_callback = callback;
			_state = state;
		}

		public Channel<T> Output { get; private set; }

		public void Send(T message)
		{
			Output.Send(message);

			Complete();
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { throw new NotSupportedException("Wait handles are not supported by the channel framework"); }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		private void Complete()
		{
			_completed = true;

			_callback(this);
		}
	}
}