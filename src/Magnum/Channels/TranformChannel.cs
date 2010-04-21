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

	/// <summary>
	/// Tranforms a message from one type to another
	/// </summary>
	/// <typeparam name="TInput">Input message type</typeparam>
	/// <typeparam name="TOutput">Output message type</typeparam>
	public class TranformChannel<TInput, TOutput> :
		Channel<TInput>
	{
		private readonly Func<TInput, TOutput> _converter;
		private readonly Fiber _fiber;

		public TranformChannel(Fiber fiber, Channel<TOutput> output, Func<TInput, TOutput> converter)
		{
			_fiber = fiber;
			Output = output;
			_converter = converter;
		}

		public Channel<TOutput> Output { get; private set; }

		public void Send(TInput message)
		{
			_fiber.Enqueue(() => Output.Send(_converter(message)));
		}
	}
}