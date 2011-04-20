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
namespace Stact.Routing.Internal
{
	using MessageHeaders;


	public class ContextAdapter<TContext> :
		Activation,
		MatchHeaderCallback<RoutingContext<TContext>>
	{
		readonly Activation _activation;
		readonly MatchHeaderImpl<RoutingContext<TContext>> _match;

		public ContextAdapter(Activation activation)
		{
			_activation = activation;
			_match = new MatchHeaderImpl<RoutingContext<TContext>>(true);
		}

		public void Activate<T>(RoutingContext<T> context)
		{
			_match.Match((RoutingContext<TContext>)context, context.Body, this);
		}

		public void Body<TBody>(RoutingContext<TContext> context, TBody body)
		{
			context.CanConvertTo<Message<TBody>>(x => _activation.Activate(x));
		}

		public void Message<TBody>(RoutingContext<TContext> context, Message<TBody> message)
		{
			context.CanConvertTo<Message<TBody>>(x => _activation.Activate(x));
		}

		public void Request<TRequest>(RoutingContext<TContext> context, Request<TRequest> request)
		{
			context.CanConvertTo<Request<TRequest>>(x => _activation.Activate(x));

			context.CanConvertTo<Message<TRequest>>(x => _activation.Activate(x));
		}

		public void Response<TResponse>(RoutingContext<TContext> context, Response<TResponse> response)
		{
			context.CanConvertTo<Response<TResponse>>(x => _activation.Activate(x));

			context.CanConvertTo<Message<TResponse>>(x => _activation.Activate(x));
		}
	}
}