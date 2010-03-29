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
namespace Magnum.Web.Binding
{
	using System.Collections.Generic;
	using System.Linq;
	using MassTransit.Serialization.Custom;

	public class DictionaryObjectPropertyBinderCache<T> :
		ObjectPropertyBinderCache<T>
	{
		private readonly Dictionary<string, ObjectPropertyBinder<T>> _properties;

		public DictionaryObjectPropertyBinderCache()
		{
			_properties = new Dictionary<string, ObjectPropertyBinder<T>>();

			var properties = typeof(T).GetAllProperties()
				.Where(x => x.GetGetMethod() != null)
				.Where(x => x.GetSetMethod(true) != null)
				.Select(x => new ObjectPropertyBinder<T>(x));

			foreach (ObjectPropertyBinder<T> property in properties)
			{
				_properties.Add(property.Name, property);
			}
		}

		public bool TryGetProperty(string name, out ObjectPropertyBinder<T> propertyBinder)
		{
			return _properties.TryGetValue(name, out propertyBinder);
		}
	}
}