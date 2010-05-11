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
namespace Magnum.Serialization.FastText
{
	using System;
	using System.IO;
	using System.Linq.Expressions;

	public interface FastTextTypeSerializer
	{
		void Serialize<T>(T obj, TextWriter writer);
		T Deserialize<T>(string text);
	}

	public class FastTextTypeSerializer<T> :
		FastTextTypeSerializer,
		TypeSerializer<T>
	{
		private readonly TypeWriter<object> _serializer;
		private readonly TypeSerializer<T> _typeSerializer;
		private TypeReader<T> _deserializer;

		public FastTextTypeSerializer(TypeSerializer<T> typeSerializer)
		{
			_typeSerializer = typeSerializer;

			_serializer = CreateDefaultWriter(typeSerializer);
			_deserializer = typeSerializer.GetReader();
		}

		public void Serialize<TObject>(TObject obj, TextWriter writer)
		{
			_serializer(obj, writer.Write);
		}

		public TResult Deserialize<TResult>(string text)
		{
			return (TResult)(object)_deserializer(text);
		}

		private static TypeWriter<object> CreateDefaultWriter(TypeSerializer<T> typeSerializer)
		{
			Type inputType = typeof (object);
			Type outType = typeof (T);

			ParameterExpression input = Expression.Parameter(inputType, "i");
			UnaryExpression convert = outType.IsValueType ? Expression.Convert(input, outType) : Expression.TypeAs(input, outType);

			Expression<Func<object, T>> lambda = Expression.Lambda<Func<object, T>>(convert, new[] {input});

			Func<object, T> call = lambda.Compile();

			TypeWriter<T> serialize = typeSerializer.GetWriter();

			return (value, output) =>
				{
					T obj = call(value);

					serialize(obj, output);
				};
		}

		public TypeReader<T> GetReader()
		{
			return _typeSerializer.GetReader();
		}

		public TypeWriter<T> GetWriter()
		{
			return _typeSerializer.GetWriter();
		}
	}
}