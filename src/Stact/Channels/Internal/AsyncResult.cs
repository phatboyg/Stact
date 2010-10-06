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
namespace Stact.Channels.Internal
{
	using System;
	using System.Threading;


	/// <summary>
	/// A callback-only version of the AsyncResult callback
	/// </summary>
	public class AsyncResult :
		IAsyncResult
	{
		readonly AsyncCallback _callback;
		readonly object _state;
		volatile bool _completed;

		public AsyncResult(AsyncCallback callback, object state)
		{
			Magnum.Guard.AgainstNull(callback, "callback");

			_callback = callback;
			_state = state;
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

		protected void Complete()
		{
			_completed = true;

			_callback(this);
		}
	}
}