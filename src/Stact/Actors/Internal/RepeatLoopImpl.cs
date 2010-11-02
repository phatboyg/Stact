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
namespace Stact.Internal
{
	using System;
	using Configuration;


	public class RepeatLoopImpl :
		RepeatLoop
	{
		readonly Inbox _inbox;
		readonly PendingReceiveList _receives;

		public RepeatLoopImpl(Inbox inbox)
		{
			_inbox = inbox;

			_receives = new PendingReceiveList
				{
					_inbox.Receive<Request<Exit>>(HandleExit),
					_inbox.Receive<Kill>(message => CancelPendingReceives())
				};
		}

		public void Send<T>(T message)
		{
			_inbox.Send(message);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer)
		{
			Func<PendingReceive> receiver = null;

			SelectiveConsumer<T> wrapped = candidate =>
				{
					Consumer<T> accepted = consumer(candidate);
					if (accepted == null)
						return null;

					return message =>
						{
							accepted(message);

							receiver();
						};
				};

			receiver = () => _inbox.Receive(wrapped);

			PendingReceive pendingReceive = receiver();
			_receives.Add(pendingReceive);

			return pendingReceive;
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback)
		{
			Func<PendingReceive> receiver = null;

			SelectiveConsumer<T> wrapped = candidate =>
				{
					Consumer<T> accepted = consumer(candidate);
					if (accepted == null)
						return null;

					return message =>
						{
							accepted(message);

							receiver();
						};
				};

			Action wrappedCallback = () =>
				{
					timeoutCallback();

					receiver();
				};

			receiver = () => _inbox.Receive(wrapped, timeout, wrappedCallback);

			PendingReceive pendingReceive = receiver();
			_receives.Add(pendingReceive);

			return pendingReceive;
		}

		Consumer<Request<Exit>> HandleExit(Request<Exit> request)
		{
			return message =>
				{
					CancelPendingReceives();

					request.Respond(message.Body);
				};
		}

		void CancelPendingReceives()
		{
			_receives.CancelAll();
		}
	}
}