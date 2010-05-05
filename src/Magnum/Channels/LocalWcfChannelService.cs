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
	using System.ServiceModel;
	using Fibers;

	/// <summary>
	///   Handles the server end of a WCF channel connection
	/// </summary>
	/// <typeparam name = "T">The channel type</typeparam>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	public class LocalWcfChannelService<T> :
		LocalWcfChannel<T>
	{
		private readonly Fiber _fiber;

		public LocalWcfChannelService(Fiber fiber, Channel<T> output)
		{
			_fiber = fiber;
			Output = output;
		}

		public Channel<T> Output { get; private set; }

		public void Send(T message)
		{
			_fiber.Enqueue(() => Output.Send(message));
		}
	}
}