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
namespace Magnum.Actors.Internal
{
	using System;
	using Channels;


	public class WithinTimeoutHandler :
		WithinTimeout
	{
		readonly Inbox _inbox;
		readonly TimeSpan _timeout;
		bool _handled;
		Action _timeoutCallback = DoNothing;

		public WithinTimeoutHandler(Inbox inbox, TimeSpan timeout, Action<WithinTimeout> initializer)
		{
			_inbox = inbox;
			_timeout = timeout;

			initializer(this);
		}

		public void Receive<T>(SelectiveConsumer<T> consumer)
		{
			_inbox.Receive<T>(candidate =>
				{
					Consumer<T> accepted = consumer(candidate);
					if (accepted == null)
						return null;

					return message =>
						{
							accepted(message);

							_handled = true;
						};
				}, _timeout, HandleTimeout);
		}

		public void Otherwise(Action timeoutCallback)
		{
			_timeoutCallback = timeoutCallback;
		}

		static void DoNothing()
		{
		}

		void HandleTimeout()
		{
			if (_handled)
				return;

			_timeoutCallback();

			_handled = true;
		}
	}
}