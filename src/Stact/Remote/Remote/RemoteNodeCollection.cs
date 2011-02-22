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
	using Loopback;
	using Magnum.Extensions;
	using Magnum.Serialization;
	using ReliableMulticast;


	public class RemoteNodeCollection :
		NodeCollection
	{
		readonly FiberFactory _fiberFactory;
		readonly IDictionary<string, Node> _nodes;
		readonly IDictionary<string, ChunkWriter> _writers;
		readonly Scheduler _scheduler;
		readonly Serializer _serializer;
		RemoteNode _localNode;

		public RemoteNodeCollection(Uri inputAddress, UntypedChannel input, FiberFactory fiberFactory, Scheduler scheduler,
		                            Serializer serializer)
			: this(fiberFactory, scheduler, serializer)
		{
			CreateLocalNode(inputAddress, input);
		}

		public RemoteNodeCollection(FiberFactory fiberFactory, Scheduler scheduler, Serializer serializer)
		{
			_fiberFactory = fiberFactory;
			_scheduler = scheduler;
			_serializer = serializer;

			_nodes = new Dictionary<string, Node>();
			_writers = new Dictionary<string, ChunkWriter>();
		}

		public Node GetNode(Uri uri)
		{
			string key = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);

			Node result;
			if (_nodes.TryGetValue(key, out result))
				return result;

			ChunkWriter chunkWriter = GetWriter(uri);

			result = new RemoteNode(new ShuntChannel(), chunkWriter, _fiberFactory, _scheduler, _serializer);

			_nodes.Add(key, result);

			return result;
		}

		public void Dispose()
		{
			_nodes.Values.Each(x => x.Dispose());
			_nodes.Clear();
		}

		void CreateLocalNode(Uri uri, UntypedChannel input)
		{
			string key = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);

			ChunkWriter chunkWriter = GetWriter(uri);

			_localNode = new RemoteNode(input, chunkWriter, _fiberFactory, _scheduler, _serializer);

			try
			{
				GetReader(uri, _localNode);
			}
			catch
			{
				_localNode.Dispose();
				throw;
			}

			_nodes.Add(key, _localNode);
		}

		void GetReader(Uri uri, RemoteNode node)
		{
			if ("pgm".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase)
				|| "rm".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase))
			{
				var reader = new ReliableMulticastListener(uri, node);
				reader.Start();

				node.AddDisposable(reader);
				return;
			}

			if ("loopback".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase))
			{
				var loopback = (LoopbackReaderWriter)GetWriter(uri);

				loopback.Output = node;
				return;
			}

			throw new StactException("Unsupported local address: " + uri);
		}

		ChunkWriter GetWriter(Uri uri)
		{
			string key = uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);

			ChunkWriter result;
			if (_writers.TryGetValue(key, out result))
				return result;

			if ("pgm".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase)
				|| "rm".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase))
			{
				result = CreateReliableMulticastWriter(uri);

				_writers.Add(key, result);
				return result;
			}

			if ("loopback".Equals(uri.Scheme, StringComparison.InvariantCultureIgnoreCase))
			{
				result =  new LoopbackReaderWriter(new PoolFiber());

				_writers.Add(key, result);
				return result;
			}

			throw new StactException("Unsupported remote address: " + uri);
		}

		static ChunkWriter CreateReliableMulticastWriter(Uri uri)
		{
			var writer = new ReliableMulticastWriter(uri);
			writer.Start();

			return writer;
		}
	}
}