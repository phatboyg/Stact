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
	/// Converts an untyped channel to a typed channel, passing only messages
	/// which can be assigned to the channel type and discarding all others
	/// </summary>
	/// <typeparam name="TOutput">The output channel type</typeparam>
	public class TypedChannelAdapter<TOutput> :
		UntypedChannel
	{
		[ThreadStatic]
		static Func<object, TOutput> _convert;

		readonly Channel<TOutput> _output;

		public TypedChannelAdapter(Channel<TOutput> output)
		{
			_output = output;
		}

		public Channel<TOutput> Output
		{
			get { return _output; }
		}

		public Type OutputType
		{
			get { return typeof(TOutput); }
		}

		public void Send<T>(T message)
		{
			if (typeof(TOutput).IsAssignableFrom(typeof(T)))
				_output.Send(Convert(message));
		}

		static TOutput Convert(object obj)
		{
			if (_convert == null)
				GenerateConvertMethod();

			return _convert(obj);
		}

		static void GenerateConvertMethod()
		{
			ParameterExpression value = Expression.Parameter(typeof(object), "value");

			// value as T is slightly faster than (T)value, so if it's not a value type, use that
			UnaryExpression castValue;
			if (typeof(TOutput).IsValueType)
				castValue = Expression.Convert(value, typeof(TOutput));
			else
				castValue = Expression.TypeAs(value, typeof(TOutput));

			Expression<Func<object, TOutput>> expression = Expression.Lambda<Func<object, TOutput>>(castValue, value);

			_convert = expression.Compile();
		}
	}
}