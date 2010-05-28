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
	using Fibers;
	using Serialization;

	public class DeserializeChannel<T> :
		Channel<string>
	{
		private readonly Fiber _fiber;
		private readonly Channel<T> _output;
		private readonly Serializer _serializer;

		public DeserializeChannel(Fiber fiber, Serializer serializer, Channel<T> output)
		{
			_fiber = fiber;
			_serializer = serializer;
			_output = output;
		}

		public void Send(string message)
		{
			_fiber.Add(() =>
				{
					var msg = _serializer.Deserialize<T>(message);

					_output.Send(msg);
				});
		}
	}
}