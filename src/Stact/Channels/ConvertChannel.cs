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
	using System.Linq.Expressions;
	

	/// <summary>
	///   Tranforms a message from one type to another
	/// </summary>
	/// <typeparam name = "TInput">Input message type</typeparam>
	/// <typeparam name = "TOutput">Output message type</typeparam>
	public class ConvertChannel<TInput, TOutput> :
		Channel<TInput>
	{
		private readonly Fiber _fiber;

		public ConvertChannel(Fiber fiber, Channel<TOutput> output)
			: this(fiber, output, CreateDefaultConverter())
		{
		}

		public ConvertChannel(Fiber fiber, Channel<TOutput> output, MessageConverter<TInput, TOutput> converter)
		{
			_fiber = fiber;
			Output = output;
			Converter = converter;
		}

		public MessageConverter<TInput, TOutput> Converter { get; private set; }

		public Channel<TOutput> Output { get; private set; }

		public void Send(TInput message)
		{
			_fiber.Add(() =>
				{
					TOutput outputMessage = Converter(message);

					Output.Send(outputMessage);
				});
		}

		private static MessageConverter<TInput, TOutput> CreateDefaultConverter()
		{
			Type inputType = typeof (TInput);
			Type outType = typeof (TOutput);

			var input = Expression.Parameter(inputType, "input");
			var body = inputType.IsValueType ? Expression.Convert(input, outType) : Expression.TypeAs(input, outType);
			var lambda = Expression.Lambda<MessageConverter<TInput, TOutput>>(body, new[] {input});

			return lambda.Compile();
		}
	}
}