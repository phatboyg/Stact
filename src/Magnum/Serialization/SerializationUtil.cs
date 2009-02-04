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
	using System.IO;
	using System.Linq.Expressions;
	using System.Reflection;

	public static class SerializationUtil<T> where T : new()
	{
		private static readonly Action<ISerializationReader, T> _deserializer;
		private static readonly Type[] _types = new Type[] {};

		public static readonly Func<T> New;
		public static readonly Action<ISerializationWriter, T> Serialize;

		static SerializationUtil()
		{
			New = BuildNewT();
			Serialize = BuildSerializer();

			_deserializer = BuildDeserializer();
		}

		public static T Deserialize(ISerializationReader reader)
		{
			try
			{
				T t = New();

				_deserializer(reader, t);

				return t;
			}
			catch (EndOfStreamException ex)
			{
				return default(T);
			}
		}

		private static Func<T> BuildNewT()
		{
			Type type = typeof (T);

			ConstructorInfo ci = type.GetConstructor(_types);

			return Expression.Lambda<Func<T>>(Expression.New(ci)).Compile();
		}

		private static Action<ISerializationWriter, T> BuildSerializer()
		{
			Action<ISerializationWriter, T> serializer = null;

			Type type = typeof (T);

			var writer = Expression.Parameter(typeof (ISerializationWriter), "writer");
			var instance = Expression.Parameter(type, "instance");

			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties)
			{
				var getter = BuildGetterForProperty(instance, propertyInfo);

				//TODO: this is where ih is falling down.
				//TODO: would have to have a method for ever type know to man
				//TODO: Register types?
				MethodInfo writerInfo = typeof (ISerializationWriter).GetMethod("Write", new[] {propertyInfo.PropertyType});

				if (writerInfo == null)
					throw new Exception("Unable to output a property of type " + propertyInfo.PropertyType.FullName);

				//TODO: What is this
				var thing = Expression.Call(writer, writerInfo,
				                            new Expression[]
				                            	{Expression.Invoke(Expression.Constant(getter), new Expression[] {instance})});

				Expression<Action<ISerializationWriter, T>> doit =
					Expression.Lambda<Action<ISerializationWriter, T>>(thing, new[] {writer, instance});

				serializer += doit.Compile();
			}

			return serializer;
		}

		private static Delegate BuildGetterForProperty(ParameterExpression instance, PropertyInfo propertyInfo)
		{
			var getMethod = Expression.Call(instance, propertyInfo.GetGetMethod());
			var returnType = propertyInfo.PropertyType;

			Type getterType = typeof (Func<,>).MakeGenericType(typeof (T), returnType);

			return Expression.Lambda(getterType, getMethod, instance).Compile();
		}

		private static Action<ISerializationReader, T> BuildDeserializer()
		{
			Action<ISerializationReader, T> deserializer = null;

			Type type = typeof (T);

			var reader = Expression.Parameter(typeof (ISerializationReader), "reader");
			var instance = Expression.Parameter(type, "instance");

			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (PropertyInfo propertyInfo in properties)
			{
				MethodInfo readerInfo = typeof (ISerializationReader).GetMethod("Read" + propertyInfo.PropertyType.Name);
				if (readerInfo == null)
					throw new Exception("Unable to read a property of type " + propertyInfo.PropertyType.FullName);

				Type readerType = typeof (Func<,>).MakeGenericType(typeof (ISerializationReader), propertyInfo.PropertyType);

				var readerLambda = Expression.Lambda(readerType, Expression.Call(reader, readerInfo), new[] {reader}).Compile();

				//var newT = Expression.Lambda<Func<T>>(Expression.New(type)).Compile();

				//var setValue = Expression.Call(instance, propertyInfo.GetSetMethod(), ));

				//UnaryExpression valueCast = (!propertyInfo.PropertyType.IsValueType) ? Expression.TypeAs(value, propertyInfo.PropertyType) : Expression.Convert(value, propertyInfo.PropertyType);

				MethodInfo setMethod = propertyInfo.GetSetMethod();
				if (setMethod == null)
				{
					var accessors = propertyInfo.GetAccessors(true);
					if (accessors != null)
						foreach (MethodInfo info in accessors)
						{
							if (info.ReturnType == typeof (void))
							{
								setMethod = info;
								break;
							}
						}
				}
				if (setMethod == null)
					throw new Exception("Unable to write to property due to visibility: " + propertyInfo.Name);

				Expression<Action<ISerializationReader, T>> doit = Expression.Lambda<Action<ISerializationReader, T>>(Expression.Call(instance, setMethod, new Expression[] {Expression.Invoke(Expression.Constant(readerLambda), new[] {reader})}), new[] {reader, instance});

				deserializer += doit.Compile();
			}

			return deserializer;
		}

		public static byte[] SerializeToByteArray(T obj)
		{
			byte[] bytes;
			using (var storage = new MemoryStream())
			using (var binaryWriter = new BinaryWriter(storage))
			{
				var writer = new BinarySerializationWriter(binaryWriter);

				Serialize(writer, obj);

				binaryWriter.Flush();
				binaryWriter.Close();

				storage.Flush();
				bytes = storage.ToArray();

				storage.Close();
			}
			return bytes;
		}

		public static T DeserializeFromByteArray(byte[] bytes)
		{
			using (var output = new MemoryStream(bytes))
			using (var binaryReader = new BinaryReader(output))
			{
				var writer = new BinarySerializationReader(binaryReader);

				return Deserialize(writer);
			}
		}
	}
}