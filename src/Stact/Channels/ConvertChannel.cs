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
		[ThreadStatic]
		static MessageConverter<TInput, TOutput> _defaultConverter;

		readonly MessageConverter<TInput, TOutput> _converter;
		readonly Fiber _fiber;
		readonly Channel<TOutput> _output;

		public ConvertChannel(Fiber fiber, Channel<TOutput> output)
			: this(fiber, output, CreateDefaultConverter())
		{
		}

		public ConvertChannel(Fiber fiber, Channel<TOutput> output, MessageConverter<TInput, TOutput> converter)
		{
			_fiber = fiber;
			_output = output;
			_converter = converter;
		}

		public MessageConverter<TInput, TOutput> Converter
		{
			get { return _converter; }
		}

		public Channel<TOutput> Output
		{
			get { return _output; }
		}

		public void Send(TInput message)
		{
			_fiber.Add(() => _output.Send(_converter(message)));
		}

		static MessageConverter<TInput, TOutput> CreateDefaultConverter()
		{
			if (_defaultConverter != null)
				return _defaultConverter;

			Type inputType = typeof(TInput);
			Type outType = typeof(TOutput);

			ParameterExpression input = Expression.Parameter(inputType, "input");
			UnaryExpression body;
			if (inputType.IsValueType)
				body = Expression.Convert(input, outType);
			else
				body = Expression.TypeAs(input, outType);

			var lambda = Expression.Lambda<MessageConverter<TInput, TOutput>>(body, input);

			_defaultConverter = lambda.Compile();

			return _defaultConverter;
		}
	}
}