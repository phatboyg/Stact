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
namespace Magnum.Serialization
{
	using System;
	using System.Reflection;

	public interface PropertyTypeSerializerCache
	{
		TypeSerializer<T> GetTypeSerializer<T>(PropertyInfo property);
	}

	public interface TypeSerializerCache
	{
		TypeSerializer<T> GetTypeSerializer<T>();

		/// <summary>
		/// Returns a type serializer for a property on a type
		/// </summary>
		/// <typeparam name="T">The type of serializer to return</typeparam>
		/// <param name="property">The property that the serializer is associated with</param>
		/// <returns>A type serializer for the type</returns>
		TypeSerializer<T> GetTypeSerializer<T>(PropertyInfo property);

		void Each(Action<Type, TypeSerializer> action);
	}
}