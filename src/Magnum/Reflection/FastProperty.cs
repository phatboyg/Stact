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
namespace Magnum.Reflection
{
	using System;
	using System.Linq.Expressions;
	using System.Reflection;

	public class FastProperty
	{
		public Func<object, object> GetDelegate;
		public Action<object, object> SetDelegate;

		public FastProperty(PropertyInfo property)
		{
			Property = property;
			InitializeGet();
			InitializeSet();
		}

		public PropertyInfo Property { get; set; }

		private void InitializeSet()
		{
			var instance = Expression.Parameter(typeof (object), "instance");
			var value = Expression.Parameter(typeof (object), "value");

			// value as T is slightly faster than (T)value, so if it's not a value type, use that
			UnaryExpression instanceCast = (!Property.DeclaringType.IsValueType) ?
				Expression.TypeAs(instance, Property.DeclaringType) : Expression.Convert(instance, Property.DeclaringType);
			UnaryExpression valueCast = (!Property.PropertyType.IsValueType) ? 
				Expression.TypeAs(value, Property.PropertyType) : Expression.Convert(value, Property.PropertyType);

			SetDelegate = Expression.Lambda<Action<object, object>>(Expression.Call(instanceCast, Property.GetSetMethod(), valueCast), new[] {instance, value}).Compile();
		}

		private void InitializeGet()
		{
			var instance = Expression.Parameter(typeof (object), "instance");
			UnaryExpression instanceCast = (!Property.DeclaringType.IsValueType) ?
				Expression.TypeAs(instance, Property.DeclaringType) : Expression.Convert(instance, Property.DeclaringType);

			GetDelegate = Expression.Lambda<Func<object, object>>(Expression.TypeAs(Expression.Call(instanceCast, Property.GetGetMethod()), typeof (object)), instance).Compile();
		}

		public object Get(object instance)
		{
			return GetDelegate(instance);
		}

		public void Set(object instance, object value)
		{
			SetDelegate(instance, value);
		}
	}

	public class FastProperty<T>
	{
		public Func<T, object> GetDelegate;
		public Action<T, object> SetDelegate;

		public FastProperty(PropertyInfo property)
		{
			Property = property;
			InitializeGet();
			InitializeSet();
		}

		public PropertyInfo Property { get; set; }

		private void InitializeSet()
		{
			var instance = Expression.Parameter(typeof (T), "instance");
			var value = Expression.Parameter(typeof (object), "value");
			UnaryExpression valueCast = (!Property.PropertyType.IsValueType) ? Expression.TypeAs(value, Property.PropertyType) : Expression.Convert(value, Property.PropertyType);
			SetDelegate = Expression.Lambda<Action<T, object>>(Expression.Call(instance, Property.GetSetMethod(), valueCast), new[] {instance, value}).Compile();
		}

		private void InitializeGet()
		{
			var instance = Expression.Parameter(typeof (T), "instance");
			GetDelegate = Expression.Lambda<Func<T, object>>(Expression.TypeAs(Expression.Call(instance, Property.GetGetMethod()), typeof (object)), instance).Compile();
		}

		public object Get(T instance)
		{
			return GetDelegate(instance);
		}

		public void Set(T instance, object value)
		{
			SetDelegate(instance, value);
		}
	}

	public class FastProperty<T, P>
	{
		public Func<T, P> GetDelegate;
		public Action<T, P> SetDelegate;

		public FastProperty(PropertyInfo property)
		{
			Property = property;
			InitializeGet();
			InitializeSet();
		}

		public PropertyInfo Property { get; set; }

		private void InitializeSet()
		{
			var instance = Expression.Parameter(typeof (T), "instance");
			var value = Expression.Parameter(typeof (P), "value");
			SetDelegate = Expression.Lambda<Action<T, P>>(Expression.Call(instance, Property.GetSetMethod(), value), new[] {instance, value}).Compile();

			// roughly looks like Action<T,P> a = new Action<T,P>((instance,value) => instance.set_Property(value));
		}

		private void InitializeGet()
		{
			var instance = Expression.Parameter(typeof (T), "instance");
			GetDelegate = Expression.Lambda<Func<T, P>>(Expression.Call(instance, Property.GetGetMethod()), instance).Compile();

			// roughly looks like Func<T,P> getter = instance => return instance.get_Property();
		}

		public P Get(T instance)
		{
			return GetDelegate(instance);
		}

		public void Set(T instance, P value)
		{
			SetDelegate(instance, value);
		}
	}
}