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
	using System.Collections.Generic;

	public class ChannelBinderScope<T> :
		BinderScope
	{
		private readonly HashSet<Channel> _boundChannels = new HashSet<Channel>();
		private readonly Channel<T> _channel;

		public ChannelBinderScope(Channel<T> channel)
		{
			_channel = channel;
		}

		public void Dispose()
		{
			new RemoveChannelBinder(_boundChannels).Unbind(_channel);

			_boundChannels.Clear();
		}

		public void Add<TChannel>(Channel<TChannel> channel)
		{
			new AddChannelBinder<TChannel>(channel).BindTo(_channel);

			_boundChannels.Add(channel);
		}
	}
}