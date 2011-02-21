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
	using Magnum.Extensions;
	using Magnum.Serialization;


	public class RemoteNodeConnectionPool :
		RemoteChannelConnectionPool
	{
		readonly IDictionary<string, RemoteNode> _channels;
		readonly Scheduler _scheduler;
		readonly Serializer _serializer;

		public RemoteNodeConnectionPool(Scheduler scheduler, Serializer serializer)
		{
			_channels = new Dictionary<string, RemoteNode>();
			_scheduler = scheduler;
			_serializer = serializer;
		}

		public HeaderChannel GetRemoteChannel(Uri uri)
		{
			string key = string.Format("{0}:{1}", uri.Host, uri.Port);

			RemoteNode result;
			if (_channels.TryGetValue(key, out result))
				return result;

			result = new RemoteNode(new PoolFiber(), _scheduler, _serializer, uri);
			result.Start();

			_channels.Add(key, result);

			return result;
		}

		public void Shutdown()
		{
			_channels.Each(x => x.Value.Dispose());
			_channels.Clear();
		}
	}
}