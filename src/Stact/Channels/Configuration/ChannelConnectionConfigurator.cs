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
namespace Stact.Channels.Configuration
{
	using Internal;


	public interface ChannelConnectionConfigurator
	{
		void SetChannelConfigurator(ChannelConfigurator configurator);
	}


	/// <summary>
	/// A fluent syntax for configuration the options of a channel subscription
	/// </summary>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public interface ChannelConnectionConfigurator<TChannel>
	{
		void SetChannelConfigurator(ChannelConfigurator<TChannel> configurator);
	}
}