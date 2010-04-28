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
namespace Magnum.Channels.Configuration
{
	using System;

	/// <summary>
	/// A fluent syntax for configuration options of a channel consumer
	/// </summary>
	/// <typeparam name="TConsumer">The consumer type</typeparam>
	/// <typeparam name="TChannel">The channel type</typeparam>
	public interface ConsumerConfigurator<TConsumer, TChannel>
	{
		ConsumerConfigurator<TConsumer, TChannel> ObtainedBy(Func<TConsumer> consumerFactory);
	}
}