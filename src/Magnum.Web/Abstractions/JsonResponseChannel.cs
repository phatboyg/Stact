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
namespace Magnum.Web.Abstractions
{
	using System;
	using System.Threading;
	using Actions;
	using Channels;

	public class JsonResponseChannel<T> :
		Channel<T>,
		IAsyncResult
	{
		private readonly AsyncCallback _callback;
		private readonly ActionQueue _queue;
		private readonly object _state;
		private readonly ObjectWriter _writer;
		private volatile bool _completed;

		public JsonResponseChannel(ObjectWriter writer, ActionQueue queue, AsyncCallback callback, object state)
		{
			_writer = writer;
			_queue = queue;
			_callback = callback;
			_state = state;
		}

		public void Send(T message)
		{
			_queue.Enqueue(() =>
				{
					_writer.Write(message);

					Complete();
				});
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