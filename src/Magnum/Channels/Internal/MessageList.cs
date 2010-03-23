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
namespace Magnum.Channels.Internal
{
	using System.Collections.Generic;
	using System.Threading;

	public class MessageList<T> :
		IMessageList<T>
	{
		private readonly object _lock = new object();
		private List<T> _messages = new List<T>();

		public void Add(T message)
		{
			lock (_lock)
			{
				_messages.Add(message);

				Monitor.PulseAll(_lock);
			}
		}

		public IList<T> RemoveAll()
		{
			lock (_lock)
			{
				List<T> result = _messages;

				_messages = new List<T>();

				Monitor.PulseAll(_lock);

				return result;
			}
		}
	}
}