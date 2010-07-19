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
	using Fibers;


	public interface ChannelConfiguratorConnection
	{
		void AddChannel(Fiber fiber, Func<Fiber, UntypedChannel> channelFactory);
		void AddChannel<TChannel>(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory);

		/// <summary>
		/// The channel to which connections are being added
		/// </summary>
		UntypedChannel Channel { get; }

		/// <summary>
		/// Add a disposable item to the connection
		/// </summary>
		/// <param name="disposable"></param>
		void AddDisposable(IDisposable disposable);
	}


	public interface ChannelConfiguratorConnection<TChannel>
	{
		void AddChannel(Fiber fiber, Func<Fiber, Channel<TChannel>> channelFactory);
		void AddChannel<T>(Fiber fiber, Func<Fiber, Channel<T>> channelFactory);

		/// <summary>
		/// Add a disposable item to the connection
		/// </summary>
		/// <param name="disposable"></param>
		void AddDisposable(IDisposable disposable);
	}
}