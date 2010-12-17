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
namespace Stact.Configuration.Builders
{
	using System;
	using System.Collections.Generic;


	public class DistinctChannelBuilder<TChannel, TKey> :
		ChannelBuilder<IDictionary<TKey, TChannel>>
	{
		readonly ChannelBuilder<ICollection<TChannel>> _builder;
		readonly KeyAccessor<TChannel, TKey> _keyAccessor;

		public DistinctChannelBuilder(ChannelBuilder<ICollection<TChannel>> builder, KeyAccessor<TChannel, TKey> keyAccessor)
		{
			_builder = builder;
			_keyAccessor = keyAccessor;
		}

		public void AddChannel(Fiber fiber, Func<Fiber, Channel<IDictionary<TKey, TChannel>>> channelFactory)
		{
			_builder.AddChannel(fiber, x =>
				{
					Channel<IDictionary<TKey, TChannel>> channel = channelFactory(new SynchronousFiber());

					return new DistinctChannel<TChannel, TKey>(fiber, _keyAccessor, channel);
				});
		}

		public void AddDisposable(IDisposable disposable)
		{
			_builder.AddDisposable(disposable);
		}
	}
}