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
namespace Magnum.Common.Serialization
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class SerializationUtil<T> where T : new()
	{
		public static readonly Action<ISerializationWriter, T> Serialize;
		public static readonly Action<ISerializationReader, T> Deserialize;

		static SerializationUtil()
		{
			Serialize = BuildSerializer();

			Deserialize = BuildDeserializer();
		}

		private static Action<ISerializationWriter, T> BuildSerializer()
		{
			Action<ISerializationWriter, T> serializer = null;

			Type type = typeof (T);

			var writer = Expression.Parameter(typeof (ISerializationWriter), "writer");
			var instance = Expression.Parameter(type, "instance");

			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties)
			{
				var getValue = Expression.Call(instance, propertyInfo.GetGetMethod());

				Type getterType = typeof (Func<,>).MakeGenericType(type, propertyInfo.PropertyType);

				var getter = Expression.Lambda(getterType, getValue, instance).Compile();

				MethodInfo writerInfo = typeof (ISerializationWriter).GetMethod("Write", new[] {propertyInfo.PropertyType});
				if (writerInfo == null)
					throw new Exception("Unable to output a property of type " + propertyInfo.PropertyType.FullName);

				Expression<Action<ISerializationWriter, T>> doit = Expression.Lambda<Action<ISerializationWriter, T>>(Expression.Call(writer, writerInfo, new Expression[] {Expression.Invoke(Expression.Constant(getter), new Expression[] {instance})}), new[] {writer, instance});

				serializer += doit.Compile();
			}

			return serializer;
		}

		private static Action<ISerializationReader, T> BuildDeserializer()
		{
			Action<ISerializationReader, T> deserializer = null;

			Type type = typeof (T);

			var reader = Expression.Parameter(typeof (ISerializationReader), "reader");
			var instance = Expression.Parameter(type, "instance");

			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties)
			{
				MethodInfo readerInfo = typeof(ISerializationReader).GetMethod("Read" + propertyInfo.PropertyType.Name);
				if (readerInfo == null)
					throw new Exception("Unable to read a property of type " + propertyInfo.PropertyType.FullName);

				Type readerType = typeof (Func<,>).MakeGenericType(typeof(ISerializationReader), propertyInfo.PropertyType);

				var readerLambda = Expression.Lambda(readerType, Expression.Call(reader, readerInfo), new [] { reader } ).Compile();

				//var newT = Expression.Lambda<Func<T>>(Expression.New(type)).Compile();

				//var setValue = Expression.Call(instance, propertyInfo.GetSetMethod(), ));

				//UnaryExpression valueCast = (!propertyInfo.PropertyType.IsValueType) ? Expression.TypeAs(value, propertyInfo.PropertyType) : Expression.Convert(value, propertyInfo.PropertyType);

				Expression<Action<ISerializationReader, T>> doit = Expression.Lambda<Action<ISerializationReader, T>>(Expression.Call(instance, propertyInfo.GetSetMethod(), new Expression[] { Expression.Invoke(Expression.Constant(readerLambda), new[] { reader}) }), new[] { reader, instance });

				deserializer += doit.Compile();
			}

			return  deserializer;
		}

	}
}