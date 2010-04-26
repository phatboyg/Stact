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
	using System.Threading;

	public class ChannelAdapter<T> :
		Channel<T>
	{
		private Channel<T> _output;

		public ChannelAdapter(Channel<T> output)
		{
			_output = output;
		}

		public Channel<T> Output
		{
			get { return _output; }
		}

		public void Send(T message)
		{
			Output.Send(message);
		}

		public void ChangeOutputChannel(Channel<T> original, Channel<T> replacement)
		{
			Interlocked.CompareExchange(ref _output, replacement, original);
			if (_output != replacement)
				throw new InvalidOperationException("The channel has been modified since it was last requested");
		}
	}
}