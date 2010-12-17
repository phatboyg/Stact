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
	using System.Threading;


	public class SynchronizedChannelBuilder<TChannel> :
		ChannelBuilder<TChannel>
	{
		readonly ChannelBuilder<TChannel> _builder;
		readonly object _state;
		readonly SynchronizationContext _synchronizationContext;

		public SynchronizedChannelBuilder(ChannelBuilder<TChannel> builder, SynchronizationContext synchronizationContext,
		                                  object state)
		{
			_builder = builder;
			_synchronizationContext = synchronizationContext;
			_state = state;
		}

		public void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory)
		{
			_builder.AddChannel(fiber, x =>
				{
					Channel<TChannel> channel = channelFactory(new SynchronousFiber());

					return new SynchronizedChannel<TChannel>(fiber, channel, _synchronizationContext, _state);
				});
		}

		public void AddDisposable(IDisposable disposable)
		{
			_builder.AddDisposable(disposable);
		}
	}
}