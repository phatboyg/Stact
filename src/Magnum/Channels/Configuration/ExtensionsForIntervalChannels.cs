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
	using Configuration;


	public static class ExtensionsForIntervalChannels
	{
		/// <summary>
		/// Specifies an interval at which the consumer should be called with a collection
		/// of messages received during that period.
		/// </summary>
		/// <param name="configurator"></param>
		/// <param name="interval">The time period of each interval</param>
		/// <returns></returns>
		public static IntervalChannelConfigurator<TChannel> BufferWithTime<TChannel>(
			this ChannelConnectionConfigurator<TChannel> configurator, TimeSpan interval)
		{
			var intervalConfigurator = new IntervalChannelConfiguratorImpl<TChannel>(interval);

			configurator.SetChannelFactory(intervalConfigurator);

			return intervalConfigurator;
		}

		public static DistinctIntervalChannelConfigurator<TChannel, TKey> DistinctlyBufferWithTime<TChannel, TKey>(
			this ChannelConnectionConfigurator<TChannel> configurator, TimeSpan interval, KeyAccessor<TChannel, TKey> keyAccessor)
		{
			var intervalConfigurator = new DistinctIntervalChannelConfiguratorImpl<TChannel, TKey>(interval, keyAccessor);

			configurator.SetChannelFactory(intervalConfigurator);

			return intervalConfigurator;
		}
	}
}