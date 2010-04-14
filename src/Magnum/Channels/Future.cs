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
	/// A future object that supports asynchronous waits and channel sends, in addition to a regular complete method
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Future<T> :
		Channel<T>,
		IAsyncResult
	{
		private readonly AsyncCallback _callback;
		private readonly ManualResetEvent _event;
		private readonly object _state;
		private volatile bool _completed;

		public Future()
			: this(DefaultCallback, 0)
		{
		}

		public Future(AsyncCallback callback, object state)
		{
			Guard.AgainstNull(callback, "callback");

			_callback = callback;
			_state = state;

			_event = new ManualResetEvent(false);
		}

		public T Value { get; private set; }

		public void Send(T message)
		{
			Complete(message);
		}

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return _event; }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public void Complete(T message)
		{
			Value = message;

			_completed = true;

			_event.Set();

			_callback(this);
		}

		public bool WaitUntilCompleted(TimeSpan timeout)
		{
			return _event.WaitOne(timeout);
		}

		public bool WaitUntilCompleted(int timeout)
		{
			return _event.WaitOne(timeout);
		}

		~Future()
		{
			_event.Close();
		}

		private static void DefaultCallback(object state)
		{
		}
	}
}