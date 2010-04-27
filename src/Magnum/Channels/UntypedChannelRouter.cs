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
namespace Magnum.Channels
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Logging;

	public class UntypedChannelRouter :
		UntypedChannel
	{
		private static readonly ILogger _log = Logger.GetLogger<UntypedChannelRouter>();

		private readonly UntypedChannel[] _subscribers;

		public UntypedChannelRouter(IEnumerable<UntypedChannel> subscribers)
		{
			Guard.AgainstNull(subscribers, "subscribers");

			_subscribers = subscribers.ToArray();
		}

		public UntypedChannelRouter(UntypedChannel[] subscribers)
		{
			Guard.AgainstNull(subscribers, "subscribers");

			_subscribers = subscribers;
		}

		public UntypedChannel[] Subscribers
		{
			get { return _subscribers; }
		}

		public void Send<T>(T message)
		{
			foreach (UntypedChannel subscriber in _subscribers)
			{
				try
				{
					subscriber.Send(message);
				}
				catch (Exception ex)
				{
					_log.Error(ex, "Subscriber exception on Send");
				}
			}
		}
	}
}