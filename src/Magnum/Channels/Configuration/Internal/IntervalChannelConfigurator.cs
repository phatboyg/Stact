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
namespace Magnum.Channels.Configuration.Internal
{
	using System;
	using System.Collections.Generic;
	using Fibers;


	/// <summary>
	/// Used to configure the options on a channel that delivers messages at regular
	/// intervals
	/// </summary>
	/// <typeparam name="TChannel"></typeparam>
	public interface IntervalChannelConfigurator<TChannel> :
		ChannelConnectionConfigurator<ICollection<TChannel>>
	{
		IntervalChannelConfigurator<TChannel> ExecuteOnFiber(Fiber fiber);
		IntervalChannelConfigurator<TChannel> ExecuteOnThread();
		IntervalChannelConfigurator<TChannel> ExecuteOnProducerThread();
		IntervalChannelConfigurator<TChannel> ExecuteOnThreadPoolFiber();
		IntervalChannelConfigurator<TChannel> UseFiberFactory(FiberFactory fiberFactory);

		IntervalChannelConfigurator<TChannel> UsePrivateScheduler();
		IntervalChannelConfigurator<TChannel> UseScheduler(Scheduler scheduler);
		IntervalChannelConfigurator<TChannel> WithSchedulerFactory(Func<Scheduler> schedulerFactory);
	}
}