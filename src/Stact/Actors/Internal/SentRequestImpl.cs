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
	using System.Linq;
	using Configuration;
	using Magnum.Extensions;
	using Magnum.Reflection;


	/// <summary>
	///   A decorator for sent requests that enables method chaining
	/// </summary>
	/// <typeparam name = "TRequest"></typeparam>
	public class SentRequestImpl<TRequest> :
		SentRequest<TRequest>
	{
		readonly Inbox _inbox;
		readonly Request<TRequest> _request;

		public SentRequestImpl(Request<TRequest> request, Inbox inbox)
		{
			_request = request;
			_inbox = inbox;
		}

		public SentRequestImpl(Inbox inbox)
		{
			_inbox = inbox;
		}

		public TRequest Body
		{
			get { return _request.Body; }
		}

		public string RequestId
		{
			get { return _request.RequestId; }
		}

		public void Send<T>(T message)
		{
			_inbox.Send(message);
		}

		public ChannelConnection Connect(Action<ConnectionConfigurator> subscriberActions)
		{
			return _inbox.Connect(subscriberActions);
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer)
		{
			return _inbox.Receive(CreateFilteredConsumer(consumer));
		}

		public PendingReceive Receive<T>(SelectiveConsumer<T> consumer, TimeSpan timeout, Action timeoutCallback)
		{
			return _inbox.Receive(CreateFilteredConsumer(consumer), timeout, timeoutCallback);
		}

		SelectiveConsumer<T> CreateFilteredConsumer<T>(SelectiveConsumer<T> consumer)
		{
			if (!typeof(T).Implements(typeof(Response<>)))
				return consumer;

			var types = new[]{typeof(TRequest), typeof(T), typeof(T).GetDeclaredGenericArguments().Single()};
			var args = new object[] {RequestId};
			var responseFilter = (ResponseFilter<T>)FastActivator.Create(typeof(RequestIdFilter<,>), types, args);

			SelectiveConsumer<T> result =  candidate =>
				{
					if (!responseFilter.Accept(candidate))
						return null;

					return consumer(candidate);
				};

			return result;
		}


		class RequestIdFilter<T, TBody> :
			ResponseFilter<T>
			where T : Response<TBody>
		{
			readonly string _requestId;

			public RequestIdFilter(string requestId)
			{
				_requestId = requestId;
			}

			public bool Accept(T message)
			{
				if (_requestId == null)
					return message.RequestId == null;

				return _requestId.Equals(message.RequestId);
			}
		}


		interface ResponseFilter<T>
		{
			bool Accept(T message);
		}
	}
}