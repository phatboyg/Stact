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
namespace Stact
{
	using System;
	using Internal;


	/// <summary>
	/// This is a type of siphon filter that would remove a message from an untyped
	/// channel rather than passing it to the output channel. It is mainly designed for handling
	/// intercept messages such as a Kill command to an actor.
	/// </summary>
	/// <typeparam name="TOutput">The message type to siphon from the channel</typeparam>
	public class UntypedFilterChannel<TOutput> :
		UntypedChannel
	{
		readonly SelectiveConsumer<TOutput> _filter;
		readonly UntypedChannel _output;

		public UntypedFilterChannel(UntypedChannel output, SelectiveConsumer<TOutput> filter)
		{
			_output = output;
			_filter = filter;
		}

		public UntypedChannel Output
		{
			get { return _output; }
		}

		public Type FilterType
		{
			get { return typeof(TOutput); }
		}

		public void Send<T>(T message)
		{
			bool filtered = HeaderTypeAdapter<TOutput>.TryConvertEx(message, x =>
			{
				Consumer<TOutput> consumer = _filter(x);
				if (consumer == null)
					return false;

				consumer(x);

				return true;
			});

			if (filtered)
				return;

			_output.Send(message);
		}
	}
}