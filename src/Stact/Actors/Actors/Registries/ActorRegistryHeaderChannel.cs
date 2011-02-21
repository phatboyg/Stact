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
namespace Stact.Actors.Registries
{
	using System;
	using MessageHeaders;
	using Stact.Internal;


	public class ActorRegistryHeaderChannel :
		UntypedChannel,
		MatchHeaderCallback
	{
		readonly ThreadSingleton<MatchHeaderCallback, MatchHeader> _match;
		readonly ActorRegistry _registry;

		public ActorRegistryHeaderChannel(ActorRegistry registry)
		{
			_registry = registry;
			_match = new ThreadSingleton<MatchHeaderCallback, MatchHeader>(() => new MatchHeaderImpl());
		}

		public void Body<TBody>(TBody body)
		{
			Console.WriteLine("Unable to route a raw message, sorry");
		}

		public void Message<TBody>(Message<TBody> message)
		{
			if (message.DestinationAddress == null)
			{
				Console.WriteLine("No destination");
				return;
			}

			var urn = new ActorUrn(message.DestinationAddress);
			Guid key = urn.GetId();

			_registry.Get(key, actor => actor.Send(message), () =>
				{
					Console.WriteLine("Actor not found: " + message.DestinationAddress);
				});
		}

		public void Request<TRequest>(Request<TRequest> request)
		{
			if (request.DestinationAddress == null)
			{
				Console.WriteLine("No destination");
				return;
			}

			var urn = new ActorUrn(request.DestinationAddress);
			Guid key = urn.GetId();

			_registry.Get(key, actor => actor.Send(request), () =>
				{
					Console.WriteLine("Actor not found: " + request.DestinationAddress);
				});
		}

		public void Response<TResponse>(Response<TResponse> response)
		{
			if (response.DestinationAddress == null)
			{
				Console.WriteLine("No destination");
				return;
			}

			var urn = new ActorUrn(response.DestinationAddress);
			Guid key = urn.GetId();

			_registry.Get(key, actor => actor.Send(response), () =>
				{
					Console.WriteLine("Actor not found: " + response.DestinationAddress);
				});
		}

		public void Send<T>(T message)
		{
			_match.Value.Match(message, this);
		}
	}
}