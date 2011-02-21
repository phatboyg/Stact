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


	public class RemoteActor :
		UntypedChannel,
		HeaderChannel
	{
		readonly Uri _actorUrn;
		readonly string _destinationAddress;
		readonly MatchHeaderChannel _match;
		readonly HeaderChannel _output;

		public RemoteActor(HeaderChannel output, Uri remoteAddress)
		{
			_output = output;
			_match = new MatchHeaderChannel(this);

			string idString = remoteAddress.AbsolutePath.Trim('/', '\\');

			Console.WriteLine("Creatin actor: " + idString);

			var actorId = new Guid(idString);
			_actorUrn = new Uri("urn:actor:uuid:" + actorId.ToString("N"));
			_destinationAddress = _actorUrn.ToString();
		}

		public void Send<T>(T message, IDictionary<string, string> headers)
		{
			headers[HeaderKey.DestinationAddress] = _destinationAddress;

			_output.Send(message, headers);
		}

		public void Send<T>(T message)
		{
			_match.Send(message);
		}
	}
}