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
namespace Stact.Remote
{
	using System;
	using System.Collections.Generic;
	using Magnum.Serialization;


	public class RemoteRegistryNode :
		RegistryNode
	{
		readonly IDictionary<string, RemoteActor> _actors;
		readonly NodeCollection _nodes;
		bool _disposed;

		public RemoteRegistryNode(ActorRegistry registry, Uri listenUri, FiberFactory fiberFactory, Scheduler scheduler,
		                          Serializer serializer)
		{
			_nodes = new RemoteNodeCollection(listenUri, registry, fiberFactory, scheduler, serializer);
			_actors = new Dictionary<string, RemoteActor>();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public ActorRef Select(Uri actorAddress)
		{
			string key = actorAddress.GetComponents(UriComponents.HttpRequestUrl, UriFormat.Unescaped);

			RemoteActor result;
			if (_actors.TryGetValue(key, out result))
				return result;

			Node node = _nodes.GetNode(actorAddress);

			result = new RemoteActor(node, actorAddress);

			_actors.Add(key, result);

			return result;
		}

		~RemoteRegistryNode()
		{
			Dispose(false);
		}

		void Dispose(bool disposing)
		{
			if (_disposed)
				return;
			if (disposing)
				_nodes.Dispose();

			_disposed = true;
		}
	}
}