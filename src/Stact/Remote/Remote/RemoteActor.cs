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
namespace Stact.Remote
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A reference to a remote actor
	/// 
	/// Forwards messages to the remote actor using the header channel specified in the constructor.
	/// </summary>
	public class RemoteActor :
		ActorInstance
	{
		readonly string _destinationAddress;
		readonly MatchHeaderChannel _output;

		public RemoteActor(HeaderChannel output, Uri remoteAddress)
		{
			_destinationAddress = ActorUrn.CreateFromRemoteAddress(remoteAddress).ToString();

			HeaderChannel headerChannel = new DestinationHeaderChannel(output, _destinationAddress);

			_output = new MatchHeaderChannel(headerChannel);
		}

		public void Send<T>(T message)
		{
			_output.Send(message);
		}


		class DestinationHeaderChannel :
			HeaderChannel
		{
			readonly string _destinationAddress;
			readonly HeaderChannel _output;

			public DestinationHeaderChannel(HeaderChannel output, string destinationAddress)
			{
				_output = output;
				_destinationAddress = destinationAddress;
			}

			public void Send<T>(T message, IDictionary<string, string> headers)
			{
				headers[HeaderKey.DestinationAddress] = _destinationAddress;

				_output.Send(message, headers);
			}
		}
	}
}