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
	using Fibers;
	using Reflection;
	using Serialization;

	public class DeserializeMessageEnvelopeChannel<T> :
		Channel<T>
		where T : MessageEnvelope
	{
		private readonly Fiber _fiber;
		private readonly UntypedChannel _output;
		private readonly Serializer _serializer;

		public DeserializeMessageEnvelopeChannel(Fiber fiber, Serializer serializer, UntypedChannel output)
		{
			_fiber = fiber;
			_serializer = serializer;
			_output = output;
		}

		public void Send(T message)
		{
			_fiber.Enqueue(() =>
				{
					Type messageType = Type.GetType(message.MessageType, true, true);

					this.FastInvoke(new[] {messageType}, "Deserialize", message.Body);
				});
		}

		private void Deserialize<T>(string body)
		{
			var message = _serializer.Deserialize<T>(body);

			_output.Send(message);
		}
	}
}