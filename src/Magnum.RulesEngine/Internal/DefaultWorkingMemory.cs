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
namespace Magnum.RulesEngine.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;

	public class DefaultWorkingMemory :
		IWorkingMemory
	{
		private readonly HashSet<object> _objects = new HashSet<object>();

		public void Add(params object[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				_objects.Add(items[i]);
			}
		}

		public IEnumerable<T> List<T>() where T : class
		{
			foreach (object obj in _objects.Flatten())
			{
				foreach (T instance in EnumerateProperties<T>(obj))
				{
					yield return instance;
				}
			}
		}

		private static IEnumerable<T> EnumerateProperties<T>(object obj) where T : class
		{
			var objectType = obj.GetType();
			var typeOfT = typeof (T);

			if (typeOfT.IsAssignableFrom(objectType))
				yield return obj as T;

			List<Func<object, T>> getters = new List<Func<object, T>>();

			foreach (var propertyInfo in objectType.GetProperties())
			{
				if (propertyInfo.PropertyType.IsValueType)
					continue;

				object child = propertyInfo.GetValue(obj, null);
				if (child == null) continue;

				if(child is IEnumerable)
				{
					IEnumerable enumerable = (IEnumerable) child;
					foreach (object element in enumerable)
					{
						foreach (var t in EnumerateProperties<T>(element))
						{
							yield return t;
						}
					}

					continue;
				}


				foreach (var t in EnumerateProperties<T>(child))
				{
					yield return t;
				}
			}
		}
	}

	public static class ExtensionsToWorkingMemory
	{
//		public static IEnumerable Flatten(params object[] objects)
//		{
//			return Flatten((IEnumerable) objects);
//		}

		public static IEnumerable Flatten(this IEnumerable enumerable)
		{
			foreach (object element in enumerable)
			{
				IEnumerable candidate = element as IEnumerable;
				if (candidate != null)
				{
					foreach (object nested in candidate)
					{
						yield return nested;
					}
				}
				else
				{
					yield return element;
				}
			}
		}
	}
}