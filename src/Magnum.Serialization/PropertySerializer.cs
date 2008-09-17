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
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using Common.Reflection;
	using Magnum.Common.CollectionExtensions;

	public class PropertySerializer :
		IObjectSerializer
	{
		public void Dispose()
		{
		}

		public void Serialize<T>(IObjectFormatter output, T item) where T : class, new()
		{
			output.Start();
			output.StartObject(typeof (T));

			PropertyInfo[] properties = typeof (T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (PropertyInfo propertyInfo in properties)
			{
				FastProperty<T> fp = new FastProperty<T>(propertyInfo);

				PropertyData pd = new PropertyData(propertyInfo.Name, fp.Get(item));

				output.Write(pd);
			}

			output.EndObject(typeof (T));
			output.Stop();
		}

		public T Deserialize<T>(IObjectParser input) where T : class, new()
		{
			foreach (object o in input)
			{
				return o as T;
			}

			return default(T);
		}
	}

	public class PropertyData : 
		IPropertyData
	{
		private readonly string _name;
		private readonly object _value;

		public PropertyData(string name, object value)
		{
			_name = name;
			_value = value;
		}

		public string Name
		{
			get { return _name; }
		}

		public object Value
		{
			get { return _value; }
		}
	}

	public interface IPropertyData
	{
		string Name { get; }
		object Value { get; }
	}
}