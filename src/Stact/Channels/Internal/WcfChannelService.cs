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
namespace Stact.Channels.Internal
{
	using System.ServiceModel;

	/// <summary>
	///   Handles the server end of a WCF channel connection
	/// </summary>
	/// <typeparam name = "T">The channel type</typeparam>
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
	public class WcfChannelService<T> :
		WcfChannel<T>
	{
		public WcfChannelService(Channel<T> output)
		{
			Output = output;
		}

		public Channel<T> Output { get; private set; }

		public void Send(T message)
		{
			Output.Send(message);
		}
	}
}